namespace TestUtilApp.CameraLight
{
    public class NodeConnection
    {
        public CameraLightNode Source { get; }
        public CameraLightNode Target { get; }

        public NodeConnection(CameraLightNode source, CameraLightNode target)
        {
            Source = source;
            Target = target;
        }
    }
}
