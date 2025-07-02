namespace API.Genius.Lib.Socket;

using System;
using System.Net.Sockets;
using System.Text;
using Serilog;

public class ClientSocket
{
    private int _maxBufferLength = 2048;
    private readonly string _serverIp = "";
    private readonly int _port = 0;
    private TcpClient _tcpClient;


    public ClientSocket(string serverIp, int port)
    {
        _serverIp = serverIp;
        _port = port;
        _tcpClient = new TcpClient(_serverIp, _port);
        Log.Debug("Conexão: _tcpClient conectado {0}", _tcpClient.Connected);
    }

    public void Close()
    {
        _tcpClient.Close();
        Log.Debug("Desconexão: _tcpClient conectado {0}", _tcpClient.Connected);
    }

    private byte[] AddMessageLength(byte[] data)
    {
        // Calcula o tamanho da mensagem e converte para dois bytes
        ushort messageLength = (ushort)data.Length;
        byte[] lengthBytes = BitConverter.GetBytes(messageLength);

        // Garante que os bytes de comprimento estejam na ordem correta (big-endian)
        if (BitConverter.IsLittleEndian)
        {
            Array.Reverse(lengthBytes);
        }

        // Cria um novo array para combinar os dois bytes de comprimento com os dados da mensagem
        byte[] messageBytes = new byte[lengthBytes.Length + data.Length];
        Array.Copy(lengthBytes, 0, messageBytes, 0, lengthBytes.Length);
        Array.Copy(data, 0, messageBytes, lengthBytes.Length, data.Length);

        return messageBytes;
    }

    private static ushort GetMessageLength(byte[] lengthBytes)
    {
        // Converte os dois bytes de comprimento de big-endian para ushort
        if (BitConverter.IsLittleEndian)
        {
            Array.Reverse(lengthBytes);
        }

        return BitConverter.ToUInt16(lengthBytes, 0);
    }

    public string Send(string messageToSend)
    {
        string responseData = string.Empty;

        try
        {
            // Converte a mensagem em um array de bytes e adiciona o tamanho
            byte[] data = Encoding.ASCII.GetBytes(messageToSend);
            byte[] messageBytes = AddMessageLength(data);

            // Obtém o stream de rede para enviar a mensagem
            NetworkStream stream = _tcpClient.GetStream();

            // Envia a mensagem para o servidor
            stream.Write(messageBytes, 0, messageBytes.Length);
            Log.Debug("Mensagem enviada ({0} bytes): {1}", messageBytes.Length-2, messageToSend);

            // Buffer para armazenar a resposta do servidor
            messageBytes = new byte[_maxBufferLength];

            // Lê os dois primeiros bytes para obter o tamanho da mensagem
            byte[] lengthBytes = new byte[2];
            stream.Read(lengthBytes, 0, 2);
            ushort messageLength = GetMessageLength(lengthBytes);

            // // Converte os dois bytes de comprimento de big-endian para ushort
            // if (BitConverter.IsLittleEndian)
            // {
            //     Array.Reverse(lengthBytes);
            // }
            // ushort messageLength = BitConverter.ToUInt16(lengthBytes, 0);

            // Lê o restante da mensagem com base no tamanho obtido
            int bytesRead = stream.Read(messageBytes, 0, messageLength);
            responseData = Encoding.ASCII.GetString(messageBytes, 0, bytesRead);
            Log.Debug("Resposta recebida ({0} bytes): {1}", messageLength, responseData);

            // Fecha tudo
            stream.Close();
            _tcpClient.Close();
        }
        catch (ArgumentNullException e)
        {
            Log.Error(e, $"ArgumentNullException: {e.Message}");
        }
        catch (SocketException e)
        {
            Log.Error(e, $"SocketException: {e.Message}");
        }

        return responseData;
    }
}
