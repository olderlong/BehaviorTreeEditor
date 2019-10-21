﻿using System;
using System.Collections.Generic;

namespace BehaviorTreeEditor
{
    [Serializable]
    public class GlobalVariable
    {
        private List<VariableField> m_VariableFields = new List<VariableField>();

        public List<VariableField> VariableFields
        {
            get { return m_VariableFields; }
            set { m_VariableFields = value; }
        }

        public bool Add(VariableField variable)
        {
            if (variable == null)
                throw new Exception("VariableField为空");

            if (string.IsNullOrEmpty(variable.VariableFieldName))
            {
                string msg = "变量名为空";
                MainForm.Instance.ShowMessage(msg);
                MainForm.Instance.ShowInfo(msg);
                return false;
            }

            if (m_VariableFields.Contains(variable))
            {
                MainForm.Instance.ShowMessage(string.Format("已存在变量:{0}", variable.VariableFieldName));
                return false;
            }

            m_VariableFields.Add(variable);
            return true;
        }

        public void Remove(VariableField variable)
        {
            if (variable == null)
                throw new Exception("VariableField为空");

            for (int i = 0; i < m_VariableFields.Count; i++)
            {
                VariableField variableField = m_VariableFields[i];
                if (variableField == variable || variableField.VariableFieldName == variable.VariableFieldName)
                {
                    m_VariableFields.RemoveAt(i);
                    break;
                }
            }
        }

        public void Remove(string variableName)
        {
            if (string.IsNullOrEmpty(variableName))
            {
                string msg = "变量名为空";
                MainForm.Instance.ShowMessage(msg);
                MainForm.Instance.ShowInfo(msg);
                return;
            }

            for (int i = 0; i < m_VariableFields.Count; i++)
            {
                VariableField variableField = m_VariableFields[i];
                if (variableField.VariableFieldName == variableName)
                {
                    m_VariableFields.RemoveAt(i);
                    break;
                }
            }
        }

        public VariableField Get(string variableName)
        {
            if (string.IsNullOrEmpty(variableName))
                throw new Exception("变量名为空");

            for (int i = 0; i < m_VariableFields.Count; i++)
            {
                VariableField variableField = m_VariableFields[i];
                if (variableField != null && variableField.VariableFieldName == variableName)
                    return variableField;
            }

            return null;
        }

        public bool ExistVariableStr(string variableName)
        {
            if (string.IsNullOrEmpty(variableName))
                throw new Exception("变量名为空");

            for (int i = 0; i < m_VariableFields.Count; i++)
            {
                VariableField variableField = m_VariableFields[i];
                if (variableField != null && variableField.VariableFieldName == variableName)
                    return true;
            }

            return false;
        }
    }
}