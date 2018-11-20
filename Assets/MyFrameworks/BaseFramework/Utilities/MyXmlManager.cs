using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace BaseFramework
{
    public static class MyXmlManager
    {
        /// <summary>
        /// Used To Save Anonymous XML Data
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="_object"></param>
        /// <param name="_filePath"></param>
        public static void SaveXML<T>(T _object, string _filePath)
        {
            //Open a new XML File
            var _serializer = new XmlSerializer(typeof(T));
            //dataPath = editor save
            //persistentDataPath = game save
            if(_filePath == "")
            {
                Debug.LogError("Path is empty");
                return;
            }
            using (FileStream _stream = new FileStream(_filePath, FileMode.Create))
            {
                _serializer.Serialize(_stream, _object);
            }
        }

        /// <summary>
        /// Used To Load Anonymous XML Data
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="_object"></param>
        /// <param name="_filePath"></param>
        public static T LoadXML<T>(string _filePath)
        {
            //Open a new XML File
            var _serializer = new XmlSerializer(typeof(T));
            //dataPath = editor save
            //persistentDataPath = game save
            if (File.Exists(_filePath))
            {
                using (FileStream _stream = new FileStream(_filePath, FileMode.Open))
                {
                    var _object = _serializer.Deserialize(_stream);
                    return (T)_object;
                }
            }
            else
            {
                Debug.Log($"File Path {_filePath} Doesn't Exist");
                return default(T);
            }
        }
    }
}