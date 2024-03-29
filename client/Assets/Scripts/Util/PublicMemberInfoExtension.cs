﻿using System;
using System.Collections.Generic;
using System.Reflection;

namespace Lockstep.Util {
    public class AttributeInfo {
        public readonly object attribute;
        public readonly List<PublicMemberInfo> memberInfos;
        public AttributeInfo(object attribute, List<PublicMemberInfo> memberInfos) {
            this.attribute = attribute;
            this.memberInfos = memberInfos;
        }
    }

    public class PublicMemberInfo {
        public readonly Type type;
        public readonly string name;
        public readonly AttributeInfo[] attributes;
        private readonly FieldInfo _fieldInfo;
        private readonly PropertyInfo _propertyInfo;

        public PublicMemberInfo(FieldInfo info) {
            this._fieldInfo = info;
            this.type = this._fieldInfo.FieldType;
            this.name = this._fieldInfo.Name;
            this.attributes = PublicMemberInfo.getAttributes(this._fieldInfo.GetCustomAttributes(false));
        }
        public PublicMemberInfo(PropertyInfo info) {
            this._propertyInfo = info;
            this.type = this._propertyInfo.PropertyType;
            this.name = this._propertyInfo.Name;
            this.attributes = PublicMemberInfo.getAttributes(this._propertyInfo.GetCustomAttributes(false));
        }
        public PublicMemberInfo(Type type, string name, AttributeInfo[] attributes = null) {
            this.type = type;
            this.name = name;
            this.attributes = attributes;
        }

        public object GetValue(object obj) {
            if((object)this._fieldInfo == null) {
                return this._propertyInfo.GetValue(obj, (object[])null);
            }
            return this._fieldInfo.GetValue(obj);
        }

        public void SetValue(object obj, object value) {
            if ((object)this._fieldInfo != null) {
                this._fieldInfo.SetValue(obj, value);
            } else {
                this._propertyInfo.SetValue(obj, value, (object[])null);
            }
        }

        private static AttributeInfo[] getAttributes(object[] attributes) {
            AttributeInfo[] attributeInfoArray = new AttributeInfo[attributes.Length];
            for(int idx = 0; idx < attributes.Length; ++idx) {
                object attribute = attributes[idx];
                attributeInfoArray[idx] = new AttributeInfo(attribute, attribute.GetType().GetPublicMemberInfos());
            }
            return attributeInfoArray;
        }
    }

    public static class PublicMemberInfoExtension {
        public static List<PublicMemberInfo> GetPublicMemberInfos(this Type type) {
            FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public);
            PropertyInfo[] properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);

            List<PublicMemberInfo> publicMemberInfoList = new List<PublicMemberInfo>(fields.Length + properties.Length);
            for(int idx = 0; idx < fields.Length; ++idx) {
                publicMemberInfoList.Add(new PublicMemberInfo(fields[idx]));
            }
            for(int idx = 0; idx < properties.Length; ++idx) {
                PropertyInfo info = properties[idx];
                if(info.CanRead && info.CanWrite && info.GetIndexParameters().Length == 0) {
                    publicMemberInfoList.Add(new PublicMemberInfo(info));
                }
            }
            return publicMemberInfoList;
        }

        public static void CopyPublicMemberValues(this object source, object target) {
            List<PublicMemberInfo> publicMemberInfos = source.GetType().GetPublicMemberInfos();
            for(int idx = 0; idx < publicMemberInfos.Count; ++idx) {
                var publicMemberInfo = publicMemberInfos[idx];
                publicMemberInfo.SetValue(target, publicMemberInfo.GetValue(source));
            }
        }

        public static void CopyFiledsTo(this object source, object target) {
            if(source.GetType() == target.GetType()) {
                CopyPublicMemberValues(source, target);
                return;
            }

            FieldInfo[] fields = source.GetType().GetFields(BindingFlags.Instance | System.Reflection.BindingFlags.Public);
            FieldInfo[] ofields = target.GetType().GetFields(BindingFlags.Instance | System.Reflection.BindingFlags.Public);
            var name2TargetField = new Dictionary<string, FieldInfo>();
            foreach(var field in ofields) {
                name2TargetField[field.Name] = field;
            }
            foreach (var filed in fields) {
                if (name2TargetField.TryGetValue(filed.Name, out var targetFiled)) {
                    if(targetFiled.FieldType == filed.FieldType) {
                        targetFiled.SetValue(target, filed.GetValue(source));
                    }
                }
            }
        }
    }
}
