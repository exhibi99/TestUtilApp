using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using TestUtilApp.CameraLight;
using TestUtilApp.Services;

namespace TestUtilApp.UI
{

    public partial class CameraLightControl : UserControl, IActivatable
    {
        // ── 런타임 인스턴스 (노드 ID 기준) ─────────────────────────
        private readonly Dictionary<Guid, ICamera>          _cameras     = new Dictionary<Guid, ICamera>();
        private readonly Dictionary<Guid, ILightController> _lights      = new Dictionary<Guid, ILightController>();
        private readonly Dictionary<Guid, ISerialPort>      _serialPorts = new Dictionary<Guid, ISerialPort>();

        // 우측 탭 → PictureBox 매핑 (ImageArea 노드 ID 기준)
        private readonly Dictionary<Guid, PictureBox> _previewBoxes = new Dictionary<Guid, PictureBox>();

        // ── 고정 ImageArea 노드 ──────────────────────────────────────
        private CameraLightNode _imageArea1;
        private CameraLightNode _imageArea2;

        public CameraLightControl()
        {
            InitializeComponent();
            UiTheme.Apply(this);
            SetupCanvas();
            SetupRightPanel();
        }

        public void OnActivated() { }

        // ── 초기화 ──────────────────────────────────────────────────

        private void SetupCanvas()
        {
            // 캔버스에 ImageArea 노드 2개 미리 배치 (고정)
            _imageArea1 = new CameraLightNode(NodeKind.ImageArea, "ImageArea 1", new Point(340, 60),  pinned: true);
            _imageArea2 = new CameraLightNode(NodeKind.ImageArea, "ImageArea 2", new Point(340, 200), pinned: true);

            _canvas.AddNode(_imageArea1);
            _canvas.AddNode(_imageArea2);

            // 캔버스 이벤트
            _canvas.NodeDoubleClicked  += OnNodeDoubleClicked;
            _canvas.ConnectionAdded    += OnConnectionAdded;
            _canvas.ConnectionRemoved  += OnConnectionRemoved;
            _canvas.NodeRemoved        += OnNodeRemoved;
            _canvas.NodeActionRequested += OnNodeActionRequested;
        }

        private void SetupRightPanel()
        {
            // 탭 2개 (ImageArea 1, 2)
            AddPreviewTab(_imageArea1);
            AddPreviewTab(_imageArea2);
        }

        private void AddPreviewTab(CameraLightNode area)
        {
            var tab = new TabPage(area.Label)
            {
                BackColor = Color.FromArgb(12, 14, 18),
                ForeColor = Color.FromArgb(128, 255, 255),
            };

            var pb = new PictureBox
            {
                Dock      = DockStyle.Fill,
                BackColor = Color.Black,
                SizeMode  = PictureBoxSizeMode.Zoom,
            };

            tab.Controls.Add(pb);
            _tabRight.TabPages.Add(tab);
            _previewBoxes[area.Id] = pb;
        }

        // ── 노드 더블클릭 → 설정 다이얼로그 ────────────────────────

        private void OnNodeDoubleClicked(CameraLightNode node)
        {
            switch (node.Kind)
            {
                case NodeKind.Camera:
                    using (var dlg = new CameraSettingsDialog(node.CameraConfig))
                    {
                        if (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
                            ApplyCameraConfig(node);
                    }
                    break;

                case NodeKind.COMPort:
                    using (var dlg = new ComPortSettingsDialog(node.ComPortConfig))
                    {
                        if (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
                            node.StatusText = node.ComPortConfig.PortName;
                    }
                    _canvas.RefreshNode(node);
                    break;

                case NodeKind.LightController:
                    _lights.TryGetValue(node.Id, out var ctrl);
                    using (var dlg = new LightSettingsDialog(node.LightConfig, ctrl))
                        dlg.ShowDialog(this);
                    break;

                case NodeKind.ImageArea:
                    // 해당 탭으로 포커스 이동
                    FocusImageAreaTab(node);
                    break;
            }
        }

        private void FocusImageAreaTab(CameraLightNode area)
        {
            if (_imageArea1 == area) _tabRight.SelectedIndex = 0;
            else if (_imageArea2 == area) _tabRight.SelectedIndex = 1;
        }

        // ── 연결 추가 ────────────────────────────────────────────────

        private void OnConnectionAdded(NodeConnection conn)
        {
            if (conn.Source.Kind == NodeKind.Camera && conn.Target.Kind == NodeKind.ImageArea)
                ConnectCameraToImageArea(conn.Source, conn.Target);

            else if (conn.Source.Kind == NodeKind.COMPort && conn.Target.Kind == NodeKind.LightController)
                ConnectComPortToLight(conn.Source, conn.Target);
        }

        private void ConnectCameraToImageArea(CameraLightNode camNode, CameraLightNode areaNode)
        {
            if (!_cameras.ContainsKey(camNode.Id))
                _cameras[camNode.Id] = new BaslerCamera();

            var camera = _cameras[camNode.Id];
            try
            {
                camera.Connect(camNode.CameraConfig.Identifier);
                camera.SetExposure(camNode.CameraConfig.ExposureUs);
                camera.SetTriggerMode(camNode.CameraConfig.TriggerMode);

                camNode.StatusText  = "Ready";
                camNode.IsActive    = true;
                areaNode.StatusText = $"← {camNode.Label}";
                areaNode.IsActive   = true;
            }
            catch (Exception ex)
            {
                camNode.StatusText = "Error";
                MessageBox.Show($"카메라 연결 실패: {ex.Message}", "연결 오류",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            _canvas.RefreshNode(camNode);
            _canvas.RefreshNode(areaNode);
        }

        private void ConnectComPortToLight(CameraLightNode comNode, CameraLightNode lightNode)
        {
            if (!_serialPorts.ContainsKey(comNode.Id))
                _serialPorts[comNode.Id] = new WindowsSerialPort();

            if (!_lights.ContainsKey(lightNode.Id))
                _lights[lightNode.Id] = new AltLightController(lightNode.LightConfig.Model);

            var port  = _serialPorts[comNode.Id];
            var light = _lights[lightNode.Id];
            try
            {
                port.Open(comNode.ComPortConfig.PortName, comNode.ComPortConfig.BaudRate);
                light.Connect(port);

                comNode.StatusText   = $"{comNode.ComPortConfig.PortName} Open";
                comNode.IsActive     = true;
                lightNode.StatusText = "Connected";
                lightNode.IsActive   = true;
            }
            catch (Exception ex)
            {
                comNode.StatusText = "Error";
                MessageBox.Show($"조명 연결 실패: {ex.Message}", "연결 오류",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            _canvas.RefreshNode(comNode);
            _canvas.RefreshNode(lightNode);
        }

        // ── 연결 제거 ────────────────────────────────────────────────

        private void OnConnectionRemoved(NodeConnection conn)
        {
            if (conn.Source.Kind == NodeKind.Camera && conn.Target.Kind == NodeKind.ImageArea)
                DisconnectCamera(conn.Source, conn.Target);

            else if (conn.Source.Kind == NodeKind.COMPort && conn.Target.Kind == NodeKind.LightController)
                DisconnectLight(conn.Source, conn.Target);
        }

        private void DisconnectCamera(CameraLightNode camNode, CameraLightNode areaNode)
        {
            if (_cameras.TryGetValue(camNode.Id, out var camera))
            {
                camera.StopLive();
                camera.Disconnect();
            }
            if (_previewBoxes.TryGetValue(areaNode.Id, out var pb))
                pb.Image = null;

            camNode.StatusText  = "Idle";
            camNode.IsActive    = false;
            areaNode.StatusText = "Idle";
            areaNode.IsActive   = false;
            _canvas.RefreshNode(camNode);
            _canvas.RefreshNode(areaNode);
        }

        private void DisconnectLight(CameraLightNode comNode, CameraLightNode lightNode)
        {
            if (_lights.TryGetValue(lightNode.Id, out var light))
            {
                if (light.IsConnected) light.TurnOffAll();
                light.Disconnect();
            }
            if (_serialPorts.TryGetValue(comNode.Id, out var port))
                port.Close();

            comNode.StatusText   = "Idle";
            comNode.IsActive     = false;
            lightNode.StatusText = "Idle";
            lightNode.IsActive   = false;
            _canvas.RefreshNode(comNode);
            _canvas.RefreshNode(lightNode);
        }

        // ── 노드 제거 ────────────────────────────────────────────────

        private void OnNodeRemoved(CameraLightNode node)
        {
            // 연결 해제는 ConnectionRemoved 이벤트에서 처리됨
            // 남은 인스턴스 정리
            if (_cameras.TryGetValue(node.Id, out var cam))     { cam.Dispose();  _cameras.Remove(node.Id); }
            if (_lights.TryGetValue(node.Id, out var lt))       { lt.Dispose();   _lights.Remove(node.Id); }
            if (_serialPorts.TryGetValue(node.Id, out var sp))  { sp.Dispose();   _serialPorts.Remove(node.Id); }
        }

        // ── 카메라 액션 (우클릭 메뉴) ───────────────────────────────────

        private void OnNodeActionRequested(CameraLightNode node, string action)
        {
            if (!_cameras.TryGetValue(node.Id, out var camera) || !camera.IsConnected)
            {
                MessageBox.Show("카메라가 연결되어 있지 않습니다.\n먼저 ImageArea 노드와 연결하세요.",
                    "카메라 미연결", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 연결된 ImageArea 찾기
            NodeConnection conn = null;
            foreach (var c in _canvas.Connections)
                if (c.Source == node && c.Target.Kind == NodeKind.ImageArea) { conn = c; break; }

            switch (action)
            {
                case "GrabOnce":
                    ExecuteGrabOnce(node, camera, conn?.Target);
                    break;

                case "StartLive":
                    StartLiveForNode(node, camera, conn?.Target);
                    break;

                case "StopLive":
                    camera.StopLive();
                    node.StatusText = "Ready";
                    node.IsActive   = true;
                    _canvas.RefreshNode(node);
                    break;
            }
        }

        private void ExecuteGrabOnce(CameraLightNode camNode, ICamera camera, CameraLightNode areaNode)
        {
            // 라이브 중이면 잠시 중단
            bool wasLive = camNode.StatusText == "Live";
            if (wasLive) camera.StopLive();

            camNode.StatusText = "Grabbing...";
            _canvas.RefreshNode(camNode);

            var (result, frame) = camera.GrabSingle(3000);

            if (result == GrabResult.Success && frame != null)
            {
                if (areaNode != null && _previewBoxes.TryGetValue(areaNode.Id, out var pb))
                {
                    if (!IsDisposed && !pb.IsDisposed)
                        Invoke((Action)(() => { pb.Image = frame; FocusImageAreaTab(areaNode); }));
                }
                camNode.StatusText = "Grabbed";
            }
            else
            {
                camNode.StatusText = result == GrabResult.Timeout ? "Timeout" : "Error";
            }
            _canvas.RefreshNode(camNode);

            // 라이브였으면 재개
            if (wasLive && areaNode != null)
                StartLiveForNode(camNode, camera, areaNode);
        }

        private void StartLiveForNode(CameraLightNode camNode, ICamera camera, CameraLightNode areaNode)
        {
            if (_previewBoxes.TryGetValue(areaNode?.Id ?? Guid.Empty, out var pb) == false) return;

            camera.StartLive(frame =>
            {
                if (IsDisposed || pb.IsDisposed) return;
                Invoke((Action)(() => { pb.Image = frame; }));
            });

            camNode.StatusText = "Live";
            camNode.IsActive   = true;
            _canvas.RefreshNode(camNode);
        }

        // ── 이미지 저장 ─────────────────────────────────────────────

        private void btnSave_Click(object sender, EventArgs e) => SaveCurrentPreviewImage();

        private void SaveCurrentPreviewImage()
        {
            // 현재 선택된 탭의 PictureBox 이미지 저장
            var selected = _tabRight.SelectedTab;
            if (selected == null) return;

            PictureBox pb = null;
            foreach (Control c in selected.Controls)
                if (c is PictureBox box) { pb = box; break; }

            if (pb?.Image == null)
            {
                MessageBox.Show("저장할 이미지가 없습니다.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (var dlg = new SaveFileDialog())
            {
                dlg.Title      = "이미지 저장";
                dlg.Filter     = "PNG 이미지 (*.png)|*.png|JPEG 이미지 (*.jpg)|*.jpg|BMP 이미지 (*.bmp)|*.bmp";
                dlg.FilterIndex = 1;
                dlg.DefaultExt  = "png";
                dlg.FileName    = $"capture_{DateTime.Now:yyyyMMdd_HHmmss}";

                if (dlg.ShowDialog(this) != DialogResult.OK) return;

                System.Drawing.Imaging.ImageFormat fmt;
                switch (dlg.FilterIndex)
                {
                    case 2:  fmt = System.Drawing.Imaging.ImageFormat.Jpeg; break;
                    case 3:  fmt = System.Drawing.Imaging.ImageFormat.Bmp;  break;
                    default: fmt = System.Drawing.Imaging.ImageFormat.Png;  break;
                }

                pb.Image.Save(dlg.FileName, fmt);
            }
        }

        // ── 카메라 설정 실시간 반영 ──────────────────────────────────

        private void ApplyCameraConfig(CameraLightNode node)
        {
            if (!_cameras.TryGetValue(node.Id, out var cam) || !cam.IsConnected) return;
            cam.SetExposure(node.CameraConfig.ExposureUs);
            cam.SetGain(node.CameraConfig.Gain);
            cam.SetTriggerMode(node.CameraConfig.TriggerMode);
        }

        // ── 종료 처리 ────────────────────────────────────────────────

        protected override void OnHandleDestroyed(EventArgs e)
        {
            foreach (var cam in _cameras.Values)  cam.Dispose();
            foreach (var lt  in _lights.Values)
            {
                if (lt.IsConnected) lt.TurnOffAll();
                lt.Dispose();
            }
            foreach (var sp in _serialPorts.Values) sp.Dispose();
            base.OnHandleDestroyed(e);
        }
    }
}
