/*
*        Copyright doctorRAZ 2014-2025 by Разыграев Андрей
*
*        Licensed under the Apache License, Version 2.0 (the "License");
*        you may not use this file except in compliance with the License.
*        You may obtain a copy of the License at
*
*            http://www.apache.org/licenses/LICENSE-2.0
*
*        Unless required by applicable law or agreed to in writing, software
*        distributed under the License is distributed on an "AS IS" BASIS,
*        WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
*        See the License for the specific language governing permissions and
*        limitations under the License.
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;

 
using drzTools.ViewModel;



namespace drzTools
{
    class Config
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Config"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        internal Config()
        {

            vm = new VM();
            Deserialize();

        }



        /// <summary> путь к XML </summary>
        internal static string XMLpatch => Path.Combine(Path.GetDirectoryName(DataSetWpfOpt.AsmFulPath), $"{DataSetWpfOpt.AppProductName}.config");

        static XmlSerializer FormatterXML => new XmlSerializer(typeof(VM));

        /// <summary>
        /// вьюмодель
        /// </summary>
        public VM vm;



        /// <summary>
        /// Serializes this instance.
        /// </summary>
        /// <returns></returns>
        internal bool Serialize()
        {
            try
            {
                if (File.Exists(XMLpatch))
                {
                    File.SetAttributes(XMLpatch, FileAttributes.Normal);
                }
                using (FileStream fs = new FileStream(XMLpatch, FileMode.Create))
                {
                    FormatterXML.Serialize(fs, vm);
                }

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        /// <summary>
        /// Deserializes this instance.
        /// </summary>
        /// <returns></returns>
        internal bool Deserialize()
        {
            if (File.Exists(XMLpatch))
            {
                try
                {
                    using (FileStream fs = new FileStream(XMLpatch, FileMode.Open, FileAccess.Read))
                    {
                        //_lFieldsFrmts = new List<FieldsFrmt>();
                        vm = FormatterXML.Deserialize(fs) as VM;
                    }
                    return true;
                }
                catch (Exception e)
                {                   
                    return Serialize();
                }
            }
            else
            {
                //сохраняем настройки по дефолту
                return Serialize();
            }
        }
    }
}
