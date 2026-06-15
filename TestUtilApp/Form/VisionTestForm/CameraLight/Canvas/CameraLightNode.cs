using System;
using System.Drawing;

namespace TestUtilApp.CameraLight
{
    public enum NodeKind { Camera, COMPort, LightController, ImageArea }

    // ── 노드별 설정 ──────────────────────────────────────────────────────────

    public class CameraConfig
    {
        public string      Identifier  = "";
        public double      ExposureUs  = 5000;
        public double      Gain        = 1.0;
        public TriggerMode TriggerMode = TriggerMode.Software;
    }

    public class ComPortConfig
    {
        public string PortName = "COM1";
        public int    BaudRate = 9600;
    }

    public enum AltModel { E8RS, E16RS }

    public class LightConfig
    {
        public AltModel Model        = AltModel.E8RS;
        public int      ChannelCount => Model == AltModel.E16RS ? 16 : 8;
    }

    // ── 노드 ─────────────────────────────────────────────────────────────────

    public class CameraLightNode
    {
        public Guid    Id      { get; }         = Guid.NewGuid();
        public NodeKind Kind   { get; }
        public string   Label  { get; set; }
        public Point    Position { get; set; }
        public bool     IsPinned { get; }       // ImageArea 는 삭제 불가

        // 설정 (해당 Kind 만 사용)
        public CameraConfig  CameraConfig  { get; } = new CameraConfig();
        public ComPortConfig ComPortConfig { get; } = new ComPortConfig();
        public LightConfig   LightConfig  { get; } = new LightConfig();

        // 런타임 표시
        public string StatusText { get; set; } = "Idle";
        public bool   IsActive   { get; set; } = false;

        // 포트 존재 여부
        public bool HasOutputPort => Kind == NodeKind.Camera || Kind == NodeKind.COMPort;
        public bool HasInputPort  => Kind == NodeKind.ImageArea || Kind == NodeKind.LightController;

        public bool CanConnectTo(CameraLightNode target)
        {
            if (Kind == NodeKind.Camera        && target.Kind == NodeKind.ImageArea)       return true;
            if (Kind == NodeKind.COMPort       && target.Kind == NodeKind.LightController) return true;
            return false;
        }

        public CameraLightNode(NodeKind kind, string label, Point position, bool pinned = false)
        {
            Kind     = kind;
            Label    = label;
            Position = position;
            IsPinned = pinned;
        }
    }
}
