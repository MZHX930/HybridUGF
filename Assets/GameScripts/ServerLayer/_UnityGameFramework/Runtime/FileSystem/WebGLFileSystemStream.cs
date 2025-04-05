using System;
using System.IO;
using System.Net;
using GameFramework;
using GameFramework.FileSystem;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityGameFramework.Runtime
{
    /// <summary>
    /// WebGL文件系统流
    /// </summary>
    public sealed class WebGLFileSystemStream : FileSystemStream, IDisposable
    {
        private readonly Stream m_Stream;

        public WebGLFileSystemStream(string fullPath, FileSystemAccess access, bool createNew)
        {
            UnityEngine.Debug.Log($"gameUpdate WebGLFileSystemStream ctor fullPath={fullPath} ; access={access.ToString()} ; createNew={createNew}");
            using (WebClient client = new WebClient())
            {
                switch (access)
                {
                    case FileSystemAccess.Read:
                        //m_Stream = client.OpenRead(fullPath);
                        break;
                    default:
                        break;
                }
                UnityEngine.Debug.Log($"gameUpdate WebGLFileSystemStream ctor end！！！");
            }
        }

        protected override long Position
        {
            get
            {
                throw new GameFrameworkException("Get position is not supported in WebGLFileSystemStream.");
            }
            set { Seek(value, SeekOrigin.Begin); }
        }

        protected override long Length
        {
            get
            {
                Debug.Log($"WebGLFileSystemStream Length={m_Stream.Length}");
                return m_Stream.Length;
            }
        }

        public void Dispose()
        {
            Debug.LogError($"WebGLFileSystemStream Dispose()");
            if (null != m_Stream)
            {
                m_Stream.Dispose();
            }
        }

        protected override void Close()
        {
            Debug.LogError($"WebGLFileSystemStream Close()");
            m_Stream.Close();
            m_Stream.Dispose();
        }

        protected override void Flush()
        {
            Debug.LogError($"WebGLFileSystemStream Flush()");
            throw new GameFrameworkException("Flush is not supported in WebGLFileSystemStream.");
        }

        protected override int Read(byte[] buffer, int startIndex, int length)
        {
            Debug.LogError($"WebGLFileSystemStream Read startIndex={startIndex} ; length={length}");

            return m_Stream.Read(buffer, startIndex, length);
        }

        protected override int ReadByte()
        {
            Debug.LogError($"WebGLFileSystemStream ReadByte()");
            return m_Stream.ReadByte();
        }

        protected override void Seek(long offset, SeekOrigin origin)
        {
            Debug.LogError($"WebGLFileSystemStream Seek() offset={offset}");
            m_Stream.Seek(offset, origin);
        }

        protected override void SetLength(long length)
        {
            Debug.LogError($"WebGLFileSystemStream SetLength() length={length}");
            throw new GameFrameworkException("SetLength is not supported in WebGLFileSystemStream.");
        }

        protected override void Write(byte[] buffer, int startIndex, int length)
        {
            throw new GameFrameworkException("Write is not supported in WebGLFileSystemStream.");
        }

        protected override void WriteByte(byte value)
        {
            Debug.LogError($"WebGLFileSystemStream WriteByte()");
            throw new GameFrameworkException("WriteByte is not supported in WebGLFileSystemStream.");
        }
    }
}
