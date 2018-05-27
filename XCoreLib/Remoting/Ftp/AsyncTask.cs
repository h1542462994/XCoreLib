using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using XCore.Component;

namespace XCore.Remoting.Ftp
{
    /// <summary>
    /// 表示一种可以暂停并继续的操作.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TInfo"></typeparam>
    public abstract class AsyncTask<TKey, TInfo> 
    {
        /// <summary>
        /// 表示操作的核心结构体,重写必须完成可暂停功能,示例可参考<see cref="FtpDownloadFileTask"/>.
        /// </summary>
        public abstract void Doing();
        /// <summary>
        /// 以异步的方式执行,注意:只有在<see cref="ProcessState"/>为<see cref="ProcessState.Waiting"/>才能启用操作.
        /// </summary>
        public async void StartAsync()
        {
            if (processState == ProcessState.Waiting)
            {
                try
                {
                    await Task.Run(() => Doing());
                }
                catch (Exception)
                {
                    ProcessState = ProcessState.Failed;
                }
            }
        }
        /// <summary>
        /// 以同步方式操作.
        /// </summary>
        public void Start()
        {
            if (processState == ProcessState.Waiting)
            {
                try
                {
                    Doing();
                }
                catch (Exception)
                {
                    ProcessState = ProcessState.Failed;
                }
            }
        }
        /// <summary>
        /// 将<see cref="isCanceling"/>指定为true,以等待停止操作.
        /// </summary>
        public void Stop()
        {
            isCanceling = true;
        }
        ProcessState processState;

        protected AsyncTask(TKey key)
        {
            Key = key;
            processState = ProcessState.Waiting;
        }
        protected AsyncTask()
        {
        }
        protected bool isCanceling = false;
        /// <summary>
        /// 标记任务执行状况,注意:必须由<see cref="Doing"/>来完成操作.
        /// </summary>
        [XmlIgnore]
        public ProcessState ProcessState
        {
            get
            {
                return processState;

            }
            protected set
            {
                processState = value;
                ProcessChanged?.Invoke(this, new ProcessChangedEventArgs<TInfo>(value, Info));
            }
        }
        /// <summary>
        /// 表示一个可识别的对象.
        /// </summary>
        public TKey Key { get; set; }
        /// <summary>
        /// 表示任务所依赖的进度信息.
        /// </summary>
        public TInfo Info { get; set; }
        /// <summary>
        /// 表示进度发生更改的事件,可通过set<see cref="ProcessState"/>触发事件.
        /// </summary>
        public event ProcessChangedEventHandler<TInfo> ProcessChanged;
    }
    public class ProcessChangedEventArgs<T> : EventArgs
    {
        public ProcessChangedEventArgs(ProcessState processState, T info)
        {
            ProcessState = processState;
            Info = info;
        }

        public ProcessState ProcessState { get; set; }
        public T Info { get; set; }
    }
    public delegate void ProcessChangedEventHandler<T>(object sender, ProcessChangedEventArgs<T> e);
    public enum ProcessState
    {
        Waiting,
        Doing,
        Completed,
        Failed
    }

}
