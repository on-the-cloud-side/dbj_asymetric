using System;
using System.Threading.Channels;
using System.Threading.Tasks;


namespace dbj;

using static DBJLog;

class ThreadChannelsSpecimen
{

    // this is just a demo
    // using byte[OxFF] as a message type is efficient
    // and not very logical inside the same demo process
    private static readonly short MSG_SIZE = 0xFF;
    static byte[] payload_ = new byte[MSG_SIZE];

    static string payload_string(byte[] pload_)
    {
        if (pload_.Length > MSG_SIZE)
            throw new Exception("payload argument length > ThreadChannelsSpecimen.MSG_SIZE");

        return System.Text.Encoding.UTF8.GetString(pload_);
    }
    static byte[] string_payload(string text_)
    {
        if (text_.Length > MSG_SIZE)
            throw new Exception("text argument length > ThreadChannelsSpecimen.MSG_SIZE");

        return System.Text.Encoding.UTF8.GetBytes(text_);
    }
    //-------------------------------------------------------------------------------------
    // whole demo is inside one function
    public static async Task Demo()
    {
        // this is unrealistic scenario in which two threads use the same AssymetricActor 
        // we use it here for assymetric encryption decription inside a same process
        // which makes no much sense from the security POV
        dbj.AssymtericActor interlocutor_ = new dbj.AssymtericActor();

        // Create a channel with byte[0xFF] values
        var channel = Channel.CreateUnbounded<byte[]>();

        // Start a consumer thread
        var consumerTask = Task.Run(async () =>
        {
            while (await channel.Reader.WaitToReadAsync())
            {
                // we receive encrypted byte[]
                while (channel.Reader.TryRead(out byte[]? item))
                {
                    var clear_text_message_arrived_ = interlocutor_.DecryptFromInterlocutor(payload_string(item));
                    // never ever do this, especialy when logging
                    // but Serilog is not building up a memory consumption when logging
                    // and this is just a demo ;)
                    info($"Message Arrived translated to a clear text: {clear_text_message_arrived_}");
                }
            }
        });

        // Start a producer thread
        var producerTask = Task.Run(async () =>
        {
            // we will send 10 encrypted messages
            for (int i = 0; i < 10; i++)
            {
                var text_to_send_ = string.Format("This is message No: {0}", i);
                var (text_to_send_encrypted_, error_) = interlocutor_.EncryptForInterlocutor(text_to_send_);
                // send it as byte[]
                await channel.Writer.WriteAsync(string_payload(text_to_send_encrypted_ ?? "text_to_send_encrypted_ was null in Sending?"));
                info($"Sent : {text_to_send_}");
                await Task.Delay(100);
            }
            // signal to channel we are done here
            channel.Writer.Complete();
        });

        // Wait for both threads to complete
        await Task.WhenAll(consumerTask, producerTask);

        info( DBJcore.Whoami() + "() -- Thread Channels Demo finished ...");
        await Task.Delay(100);
    }
}
