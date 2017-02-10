//======================================================================
//
//        Copyright (C)  1996-2012  lfz    
//        All rights reserved
//
//        Filename : Singleton
//        DESCRIPTION : 获取类的单例
//
//        Created By 林芳崽 at  2013-01-08 09:11:01
//        https://git.oschina.net/lfz/tools
//
//======================================================================   

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Lfz.Utitlies
{
    /// <summary>
    /// A statically compiled "singleton" used to store objects throughout the 
    /// lifetime of the app domain. Not so much singleton in the pattern's 
    /// sense of the word as a standardized way to store single instances.
    /// </summary>
    /// <typeparam name="T">The type of object to store.</typeparam>
    /// <remarks>Access to the instance is not synchrnoized.</remarks>
    public class Singleton<T> : Singleton
    {
        static T _instance;

        /// <summary>The singleton instance for the specified type T. Only one instance (at the time) of this object for each type of T.</summary>
        public static T Instance
        {
            get { return _instance; }
            set
            {
                _instance = value;
                AllSingletons[typeof(T)] = value;
            }
        }
    }

    /// <summary>
    /// 默认单例实现
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DefaultSingleton<T> : Singleton<T> where T : class ,new()
    {
        static object lockObj = new object();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static T GetInstance()
        {
            if (Instance == null)
            {
                lock (lockObj)
                {
                    if (Instance == null)
                    {
                        Instance = new T();
                        return Instance;
                    }
                }
            }
            return Instance;
        }
        /// <summary>
        /// 获取单例实例。
        /// <para>　　如果实例已经存在，那么从单例列表中获取；如果单例实例列表中不存在，那么根据func函数构造一个新的单例，并加入列表且返回。</para>
        /// </summary>
        /// <returns>如果实例已经存在，那么从单例列表中获取；如果单例实例列表中不存在，那么根据func函数构造一个新的单例，并加入列表且返回。</returns>
        public static T GetInstance(Func<T>  func)
        {
            if (Instance == null)
            {
                lock (lockObj)
                {
                    if (Instance == null)
                    {
                        Instance = func();
                        return Instance;
                    }
                }
            }
            return Instance;
        }
    }

    /// <summary>
    /// Provides a singleton list for a certain type.
    /// </summary>
    /// <typeparam name="T">The type of list to store.</typeparam>
    public class SingletonList<T> : Singleton<IList<T>>
    {
        static SingletonList()
        {
            Singleton<IList<T>>.Instance = new List<T>();
        }

        /// <summary>
        /// The singleton instance for the specified type T. Only one instance (at the time) of this list for each type of T.
        /// </summary>
        public new static IList<T> Instance
        {
            get { return Singleton<IList<T>>.Instance; }
        }
    }

    /// <summary>
    /// Provides a singleton dictionary for a certain key and vlaue type.
    /// </summary>
    /// <typeparam name="TKey">The type of key.</typeparam>
    /// <typeparam name="TValue">The type of value.</typeparam>
    public class SingletonDictionary<TKey, TValue> : Singleton<IDictionary<TKey, TValue>>
    {
        static SingletonDictionary()
        {
            Singleton<ConcurrentDictionary<TKey, TValue>>.Instance = new ConcurrentDictionary<TKey, TValue>();
        }

        /// <summary>
        /// The singleton instance for the specified type T. Only one instance (at the time) of this dictionary for each type of T.
        /// </summary>
        public new static IDictionary<TKey, TValue> Instance
        {
            get { return Singleton<ConcurrentDictionary<TKey, TValue>>.Instance; }
        }
    }

    /// <summary>
    /// Provides access to all "singletons" stored by <see cref="Singleton{T}"/>.
    /// </summary>
    public class Singleton
    {
        static Singleton()
        {
            _allSingletons = new ConcurrentDictionary<Type, object>();
        }

// ReSharper disable InconsistentNaming
        static readonly IDictionary<Type, object> _allSingletons;
// ReSharper restore InconsistentNaming

        /// <summary>
        /// 所有单例对象列表
        /// </summary>
        public static IDictionary<Type, object> AllSingletons
        {
            get { return _allSingletons; }
        }
    }
}