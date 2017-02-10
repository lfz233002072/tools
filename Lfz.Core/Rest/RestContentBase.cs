using System.ComponentModel;

namespace Lfz.Rest
{
    /// <summary>
    /// 支持Rest服务的内容主体基类（可JSON序列化）
    /// </summary> 
    public class RestContentBase : IJsonContent, INotifyPropertyChanged, INotifyPropertyChanging
    {  
        /// <summary>
        /// 属性修改完成处理事件
        /// </summary>
        public virtual event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 激活属性修改完成处理事件
        /// </summary>
        /// <param name="propertyName"></param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual event PropertyChangingEventHandler PropertyChanging;

        /// <summary>
        /// 激活属性修改完成处理事件
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="newValue"></param>
        protected virtual void OnPropertyChanging(string propertyName, string newValue)
        {
            PropertyChangingEventHandler handler = PropertyChanging;
            if (handler != null) handler(this, new CustomPropertyChangingEventArgs<string>(propertyName, newValue));
        }
    }

    /// <summary>
    /// 属性修改事件自定义参数
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CustomPropertyChangingEventArgs<T> : PropertyChangingEventArgs
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="newValue"></param>
        public CustomPropertyChangingEventArgs(string propertyName, T newValue)
            : base(propertyName)
        {
            NewValue = newValue;
        }

        /// <summary>
        /// 属性新值
        /// </summary>
        public T NewValue { get; set; }
    }
}