using System.Runtime.CompilerServices;
using System.Threading;

[assembly: InternalsVisibleTo("Hazel.Tests")]
namespace Hazel
{
    /// <summary>
    ///     Holds statistics about the traffic through a <see cref="Connection"/>.
    /// </summary>
    /// <threadsafety static="true" instance="true"/>
    public class ConnectionStatistics
    {
        private const int ExpectedMTU = 1200;

        /// <summary>
        ///     The total number of messages sent.
        /// </summary>
        public int MessagesSent
        {
            get
            {
                return UnreliableMessagesSent + ReliableMessagesSent + FragmentedMessagesSent + AcknowledgementMessagesSent + HelloMessagesSent;
            }
        }

        /// <summary>
        ///     The number of messages sent larger than 576 bytes. This is smaller than most default MTUs.
        /// </summary>
        /// <remarks>
        ///     This is the number of unreliable messages that were sent from the <see cref="Connection"/>, incremented 
        ///     each time that LogUnreliableSend is called by the Connection. Messages that caused an error are not 
        ///     counted and messages are only counted once all other operations in the send are complete.
        /// </remarks>
        public int FragmentableMessagesSent
        {
            get
            {
                return fragmentableMessagesSent;
            }
        }

        /// <summary>
        ///     The number of messages sent larger than 576 bytes.
        /// </summary>
        int fragmentableMessagesSent;

        /// <summary>
        ///     The number of unreliable messages sent.
        /// </summary>
        /// <remarks>
        ///     This is the number of unreliable messages that were sent from the <see cref="Connection"/>, incremented 
        ///     each time that LogUnreliableSend is called by the Connection. Messages that caused an error are not 
        ///     counted and messages are only counted once all other operations in the send are complete.
        /// </remarks>
        public int UnreliableMessagesSent
        {
            get
            {
                return unreliableMessagesSent;
            }
        }

        /// <summary>
        ///     The number of unreliable messages sent.
        /// </summary>
        int unreliableMessagesSent;

        /// <summary>
        ///     The number of reliable messages sent.
        /// </summary>
        /// <remarks>
        ///     This is the number of reliable messages that were sent from the <see cref="Connection"/>, incremented 
        ///     each time that LogReliableSend is called by the Connection. Messages that caused an error are not 
        ///     counted and messages are only counted once all other operations in the send are complete.
        /// </remarks>
        public int ReliableMessagesSent
        {
            get
            {
                return reliableMessagesSent;
            }
        }

        /// <summary>
        ///     The number of unreliable messages sent.
        /// </summary>
        int reliableMessagesSent;

        /// <summary>
        ///     The number of fragmented messages sent.
        /// </summary>
        /// <remarks>
        ///     This is the number of fragmented messages that were sent from the <see cref="Connection"/>, incremented 
        ///     each time that LogFragmentedSend is called by the Connection. Messages that caused an error are not 
        ///     counted and messages are only counted once all other operations in the send are complete.
        /// </remarks>
        public int FragmentedMessagesSent
        {
            get
            {
                return fragmentedMessagesSent;
            }
        }

        /// <summary>
        ///     The number of fragmented messages sent.
        /// </summary>
        int fragmentedMessagesSent;

        /// <summary>
        ///     The number of acknowledgement messages sent.
        /// </summary>
        /// <remarks>
        ///     This is the number of acknowledgements that were sent from the <see cref="Connection"/>, incremented 
        ///     each time that LogAcknowledgementSend is called by the Connection. Messages that caused an error are not 
        ///     counted and messages are only counted once all other operations in the send are complete.
        /// </remarks>
        public int AcknowledgementMessagesSent
        {
            get
            {
                return acknowledgementMessagesSent;
            }
        }

        /// <summary>
        ///     The number of acknowledgement messages sent.
        /// </summary>
        int acknowledgementMessagesSent;

        /// <summary>
        ///     The number of hello messages sent.
        /// </summary>
        /// <remarks>
        ///     This is the number of hello messages that were sent from the <see cref="Connection"/>, incremented 
        ///     each time that LogHelloSend is called by the Connection. Messages that caused an error are not 
        ///     counted and messages are only counted once all other operations in the send are complete.
        /// </remarks>
        public int HelloMessagesSent
        {
            get
            {
                return helloMessagesSent;
            }
        }

        /// <summary>
        ///     The number of hello messages sent.
        /// </summary>
        int helloMessagesSent;

        /// <summary>
        ///     The number of bytes of data sent.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         This is the number of bytes of data (i.e. user bytes) that were sent from the <see cref="Connection"/>, 
        ///         accumulated each time that LogSend is called by the Connection. Messages that caused an error are not 
        ///         counted and messages are only counted once all other operations in the send are complete.
        ///     </para>
        ///     <para>
        ///         For the number of bytes including protocol bytes see <see cref="TotalBytesSent"/>.
        ///     </para>
        /// </remarks>
        public long DataBytesSent
        {
            get
            {
                return Interlocked.Read(ref dataBytesSent);
            }
        }

        /// <summary>
        ///     The number of bytes of data sent.
        /// </summary>
        long dataBytesSent;

        /// <summary>
        ///     The number of bytes sent in total.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         This is the total number of bytes (the data bytes plus protocol bytes) that were sent from the 
        ///         <see cref="Connection"/>, accumulated each time that LogSend is called by the Connection. Messages that 
        ///         caused an error are not counted and messages are only counted once all other operations in the send are 
        ///         complete.
        ///     </para>
        ///     <para>
        ///         For the number of data bytes excluding protocol bytes see <see cref="DataBytesSent"/>.
        ///     </para>
        /// </remarks>
        public long TotalBytesSent
        {
            get
            {
                return Interlocked.Read(ref totalBytesSent);
            }
        }

        /// <summary>
        ///     The number of bytes sent in total.
        /// </summary>
        long totalBytesSent;

        /// <summary>
        ///     The total number of messages received.
        /// </summary>
        public int MessagesReceived
        {
            get
            {
                return UnreliableMessagesReceived + ReliableMessagesReceived + FragmentedMessagesReceived + AcknowledgementMessagesReceived + helloMessagesReceived;
            }
        }
        
        /// <summary>
        ///     The number of unreliable messages received.
        /// </summary>
        /// <remarks>
        ///     This is the number of unreliable messages that were received by the <see cref="Connection"/>, incremented
        ///     each time that LogUnreliableReceive is called by the Connection. Messages are counted before the receive event is invoked.
        /// </remarks>
        public int UnreliableMessagesReceived
        {
            get
            {
                return unreliableMessagesReceived;
            }
        }

        /// <summary>
        ///     The number of unreliable messages received.
        /// </summary>
        int unreliableMessagesReceived;

        /// <summary>
        ///     The number of reliable messages received.
        /// </summary>
        /// <remarks>
        ///     This is the number of reliable messages that were received by the <see cref="Connection"/>, incremented
        ///     each time that LogReliableReceive is called by the Connection. Messages are counted before the receive event is invoked.
        /// </remarks>
        public int ReliableMessagesReceived
        {
            get
            {
                return reliableMessagesReceived;
            }
        }

        /// <summary>
        ///     The number of reliable messages received.
        /// </summary>
        int reliableMessagesReceived;

        /// <summary>
        ///     The number of fragmented messages received.
        /// </summary>
        /// <remarks>
        ///     This is the number of fragmented messages that were received by the <see cref="Connection"/>, incremented
        ///     each time that LogFragmentedReceive is called by the Connection. Messages are counted before the receive event is invoked.
        /// </remarks>
        public int FragmentedMessagesReceived
        {
            get
            {
                return fragmentedMessagesReceived;
            }
        }

        /// <summary>
        ///     The number of fragmented messages received.
        /// </summary>
        int fragmentedMessagesReceived;

        /// <summary>
        ///     The number of acknowledgement messages received.
        /// </summary>
        /// <remarks>
        ///     This is the number of acknowledgement messages that were received by the <see cref="Connection"/>, incremented
        ///     each time that LogAcknowledgemntReceive is called by the Connection. Messages are counted before the receive event is invoked.
        /// </remarks>
        public int AcknowledgementMessagesReceived
        {
            get
            {
                return acknowledgementMessagesReceived;
            }
        }

        /// <summary>
        ///     The number of acknowledgement messages received.
        /// </summary>
        int acknowledgementMessagesReceived;

        /// <summary>
        ///     The number of ping messages received.
        /// </summary>
        /// <remarks>
        ///     This is the number of hello messages that were received by the <see cref="Connection"/>, incremented
        ///     each time that LogHelloReceive is called by the Connection. Messages are counted before the receive event is invoked.
        /// </remarks>
        public int PingMessagesReceived
        {
            get
            {
                return pingMessagesReceived;
            }
        }

        /// <summary>
        ///     The number of hello messages received.
        /// </summary>
        int pingMessagesReceived;

        /// <summary>
        ///     The number of hello messages received.
        /// </summary>
        /// <remarks>
        ///     This is the number of hello messages that were received by the <see cref="Connection"/>, incremented
        ///     each time that LogHelloReceive is called by the Connection. Messages are counted before the receive event is invoked.
        /// </remarks>
        public int HelloMessagesReceived
        {
            get
            {
                return helloMessagesReceived;
            }
        }

        /// <summary>
        ///     The number of hello messages received.
        /// </summary>
        int helloMessagesReceived;

        /// <summary>
        ///     The number of bytes of data received.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         This is the number of bytes of data (i.e. user bytes) that were received by the <see cref="Connection"/>, 
        ///         accumulated each time that LogReceive is called by the Connection. Messages are counted before the receive
        ///         event is invoked.
        ///     </para>
        ///     <para>
        ///         For the number of bytes including protocol bytes see <see cref="TotalBytesReceived"/>.
        ///     </para>
        /// </remarks>
        public long DataBytesReceived
        {
            get
            {
                return Interlocked.Read(ref dataBytesReceived);
            }
        }

        /// <summary>
        ///     The number of bytes of data received.
        /// </summary>
        long dataBytesReceived;

        /// <summary>
        ///     The number of bytes received in total.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         This is the total number of bytes (the data bytes plus protocol bytes) that were received by the 
        ///         <see cref="Connection"/>, accumulated each time that LogReceive is called by the Connection. Messages are 
        ///         counted before the receive event is invoked.
        ///     </para>
        ///     <para>
        ///         For the number of data bytes excluding protocol bytes see <see cref="DataBytesReceived"/>.
        ///     </para>
        /// </remarks>
        public long TotalBytesReceived
        {
            get
            {
                return Interlocked.Read(ref totalBytesReceived);
            }
        }

        /// <summary>
        ///     The number of bytes received in total.
        /// </summary>
        long totalBytesReceived;

        public int MessagesResent { get { return messagesResent; } }
        int messagesResent;

        /// <summary>
        ///     Logs the sending of an unreliable data packet in the statistics.
        /// </summary>
        /// <param name="dataLength">The number of bytes of data sent.</param>
        /// <param name="totalLength">The total number of bytes sent.</param>
        /// <remarks>
        ///     This should be called after the data has been sent and should only be called for data that is sent sucessfully.
        /// </remarks>
        internal void LogUnreliableSend(int dataLength, int totalLength)
        {
            Interlocked.Increment(ref unreliableMessagesSent);
            Interlocked.Add(ref dataBytesSent, dataLength);
            Interlocked.Add(ref totalBytesSent, totalLength);

            if (totalLength > ExpectedMTU)
            {
                Interlocked.Increment(ref fragmentableMessagesSent);
            }
        }

        /// <summary>
        ///     Logs the sending of a reliable data packet in the statistics.
        /// </summary>
        /// <param name="dataLength">The number of bytes of data sent.</param>
        /// <param name="totalLength">The total number of bytes sent.</param>
        /// <remarks>
        ///     This should be called after the data has been sent and should only be called for data that is sent sucessfully.
        /// </remarks>
        internal void LogReliableSend(int dataLength, int totalLength)
        {
            Interlocked.Increment(ref reliableMessagesSent);
            Interlocked.Add(ref dataBytesSent, dataLength);
            Interlocked.Add(ref totalBytesSent, totalLength);

            if (totalLength > ExpectedMTU)
            {
                Interlocked.Increment(ref fragmentableMessagesSent);
            }
        }

        /// <summary>
        ///     Logs the sending of a fragmented data packet in the statistics.
        /// </summary>
        /// <param name="dataLength">The number of bytes of data sent.</param>
        /// <param name="totalLength">The total number of bytes sent.</param>
        /// <remarks>
        ///     This should be called after the data has been sent and should only be called for data that is sent sucessfully.
        /// </remarks>
        internal void LogFragmentedSend(int dataLength, int totalLength)
        {
            Interlocked.Increment(ref fragmentedMessagesSent);
            Interlocked.Add(ref dataBytesSent, dataLength);
            Interlocked.Add(ref totalBytesSent, totalLength);

            if (totalLength > ExpectedMTU)
            {
                Interlocked.Increment(ref fragmentableMessagesSent);
            }
        }

        /// <summary>
        ///     Logs the sending of a acknowledgement data packet in the statistics.
        /// </summary>
        /// <param name="totalLength">The total number of bytes sent.</param>
        /// <remarks>
        ///     This should be called after the data has been sent and should only be called for data that is sent sucessfully.
        /// </remarks>
        internal void LogAcknowledgementSend(int totalLength)
        {
            Interlocked.Increment(ref acknowledgementMessagesSent);
            Interlocked.Add(ref totalBytesSent, totalLength);
        }

        /// <summary>
        ///     Logs the sending of a hellp data packet in the statistics.
        /// </summary>
        /// <param name="totalLength">The total number of bytes sent.</param>
        /// <remarks>
        ///     This should be called after the data has been sent and should only be called for data that is sent sucessfully.
        /// </remarks>
        internal void LogHelloSend(int totalLength)
        {
            Interlocked.Increment(ref helloMessagesSent);
            Interlocked.Add(ref totalBytesSent, totalLength);
        }

        /// <summary>
        ///     Logs the receiving of an unreliable data packet in the statistics.
        /// </summary>
        /// <param name="dataLength">The number of bytes of data received.</param>
        /// <param name="totalLength">The total number of bytes received.</param>
        /// <remarks>
        ///     This should be called before the received event is invoked so it is up to date for subscribers to that event.
        /// </remarks>
        internal void LogUnreliableReceive(int dataLength, int totalLength)
        {
            Interlocked.Increment(ref unreliableMessagesReceived);
            Interlocked.Add(ref dataBytesReceived, dataLength);
            Interlocked.Add(ref totalBytesReceived, totalLength);
        }

        /// <summary>
        ///     Logs the receiving of a reliable data packet in the statistics.
        /// </summary>
        /// <param name="dataLength">The number of bytes of data received.</param>
        /// <param name="totalLength">The total number of bytes received.</param>
        /// <remarks>
        ///     This should be called before the received event is invoked so it is up to date for subscribers to that event.
        /// </remarks>
        internal void LogReliableReceive(int dataLength, int totalLength)
        {
            Interlocked.Increment(ref reliableMessagesReceived);
            Interlocked.Add(ref dataBytesReceived, dataLength);
            Interlocked.Add(ref totalBytesReceived, totalLength);
        }

        /// <summary>
        ///     Logs the receiving of a fragmented data packet in the statistics.
        /// </summary>
        /// <param name="dataLength">The number of bytes of data received.</param>
        /// <param name="totalLength">The total number of bytes received.</param>
        /// <remarks>
        ///     This should be called before the received event is invoked so it is up to date for subscribers to that event.
        /// </remarks>
        internal void LogFragmentedReceive(int dataLength, int totalLength)
        {
            Interlocked.Increment(ref fragmentedMessagesReceived);
            Interlocked.Add(ref dataBytesReceived, dataLength);
            Interlocked.Add(ref totalBytesReceived, totalLength);
        }

        /// <summary>
        ///     Logs the receiving of an acknowledgement data packet in the statistics.
        /// </summary>
        /// <param name="totalLength">The total number of bytes received.</param>
        /// <remarks>
        ///     This should be called before the received event is invoked so it is up to date for subscribers to that event.
        /// </remarks>
        internal void LogAcknowledgementReceive(int totalLength)
        {
            Interlocked.Increment(ref acknowledgementMessagesReceived);
            Interlocked.Add(ref totalBytesReceived, totalLength);
        }

        /// <summary>
        ///     Logs the receiving of a hello data packet in the statistics.
        /// </summary>
        /// <param name="totalLength">The total number of bytes received.</param>
        /// <remarks>
        ///     This should be called before the received event is invoked so it is up to date for subscribers to that event.
        /// </remarks>
        internal void LogPingReceive(int totalLength)
        {
            Interlocked.Increment(ref pingMessagesReceived);
            Interlocked.Add(ref totalBytesReceived, totalLength);
        }

        /// <summary>
        ///     Logs the receiving of a hello data packet in the statistics.
        /// </summary>
        /// <param name="totalLength">The total number of bytes received.</param>
        /// <remarks>
        ///     This should be called before the received event is invoked so it is up to date for subscribers to that event.
        /// </remarks>
        internal void LogHelloReceive(int totalLength)
        {
            Interlocked.Increment(ref helloMessagesReceived);
            Interlocked.Add(ref totalBytesReceived, totalLength);
        }

        internal void LogMessageResent()
        {
            Interlocked.Increment(ref messagesResent);
        }
    }
}
