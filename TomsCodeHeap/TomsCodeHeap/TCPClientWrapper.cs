// ========================================================================
// File:     TCPClientWrapper.cs
//
// Author:   $Author$
// Date:     $LastChangedDate$
// Revision: $Revision$
// ========================================================================
// Copyright [2012] [$Author$]
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// ========================================================================
namespace CH.Froorider.Codeheap
{
    using System;
    using System.Net.Sockets;

    /// <summary>
    /// Wrapper around the System.Net TcpClient class for easier usage.
    /// </summary>
    public class TCPClientWrapper : IDisposable
    {
        private readonly TcpClient client;
        private readonly Byte[] receiveBuffer = new byte[1024];
        private readonly NetworkStream stream;
        private Byte[] sendBuffer;
        private Int32 receivedBytes;
        private String responseData = String.Empty;
        private bool isDisposed;

        internal TCPClientWrapper(string hostname, int port)
        {
            this.client = new TcpClient(hostname, port);
            this.stream = client.GetStream();
        }

        internal TCPClientWrapper(string hostname, int port, int bufferSize)
            : this(hostname, port)
        {
            this.receiveBuffer = new byte[bufferSize];
        }

        internal void Write(string telegram)
        {
            this.sendBuffer = System.Text.Encoding.ASCII.GetBytes(telegram);
            this.stream.Write(sendBuffer, 0, sendBuffer.Length);
        }

        internal string Read()
        {
            this.receivedBytes = this.stream.Read(receiveBuffer, 0, receiveBuffer.Length);
            this.responseData = System.Text.Encoding.ASCII.GetString(receiveBuffer, 0, receivedBytes);
            return this.responseData;
        }

        internal string ReadWrite(string telegram)
        {
            this.Write(telegram);
            return this.Read();
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="TCPClientWrapper"/> class.
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="TCPClientWrapper"/> is reclaimed by garbage collection.
        /// </summary>
        ~TCPClientWrapper()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Disposes the object.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes the managed and unmanaged ressources
        /// </summary>
        /// <param name="disposing">true if everything should be disposed. False if only the unmamaged ressources should be disposed.</param>
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called. 
            if (!this.isDisposed)
            {
                // If disposing equals true, dispose all managed 
                // and unmanaged resources. 
                if (disposing)
                {
                    // Dispose managed resources.
                    if (this.stream != null)
                    {
                        this.stream.Close();
                    }

                    if (this.client != null)
                    {
                        this.client.Close();
                    }
                }

                this.isDisposed = true;
            }
        }
    }
}
