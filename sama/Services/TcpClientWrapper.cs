using System.Net.Sockets;
using System.Threading.Tasks;

namespace sama.Services
{
    /// <summary>
    /// This is a wrapper around TcpClient, which cannot be (easily) tested.
    /// </summary>
    public class TcpClientWrapper
    {
        public virtual async Task SendData(string address, int port, byte[] data)
        {
            using (var client = new TcpClient(address, port))
            using (var stream = client.GetStream())
            {
                await stream.WriteAsync(data, 0, data.Length);
            }
        }
    }
}
