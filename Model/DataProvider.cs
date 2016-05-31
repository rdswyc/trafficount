
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Trafficount.Model
{
    public static class DataProvider
    {
        public static XNamespace ns = Constants.xmlns;

        private static string _filePath;
        private static XElement _xRoot;

        public static string FilePath
        {
            get
            {
                if (_filePath == null)
                {
                    string path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                    _filePath = Path.Combine(path, Constants.fileName);
                }
                return _filePath;
            }
        }

        public static XElement RootElement
        {
            get
            {
                if (_xRoot == null)
                {
                    if (File.Exists(FilePath))
                    {
                        _xRoot = XElement.Load(FilePath);
                    }
                    else
                    {
                        _xRoot = new XElement(ns + Constants.elemRoot);
                        _xRoot.Save(FilePath);
                    }
                }

                return _xRoot;
            }
        }

        #region Corners

        public static List<Corner> CornersInFile
        {
            get
            {
                return (from e in RootElement.Elements(ns + Constants.elemCorner)
                        select new Corner(
                            (string)e.Attribute(Constants.attribName),
                            e.Attribute(Constants.attribDesc) == null ? "" : (string)e.Attribute(Constants.attribDesc)
                        )).ToList();
            }
        }

        public static void RemoveCornerFromFile(int position)
        {
            RootElement.Elements(ns + Constants.elemCorner).ElementAt(position).Remove();
            RootElement.Save(FilePath);
        }

        public static void EditCornerInFile(int position, string name, string description = "")
        {
            XElement xCorner = RootElement.Elements(ns + Constants.elemCorner).ElementAt(position);
            xCorner.Attribute(Constants.attribName).Value = name;

            if (description == "")
            {
                if (xCorner.Attributes(Constants.attribDesc).Count() > 0)
                    xCorner.Attribute(Constants.attribDesc).Remove();
            }
            else
            {
                if (xCorner.Attributes(Constants.attribDesc).Count() == 0)
                    xCorner.Add(new XAttribute(Constants.attribDesc, description));
                else
                    xCorner.Attribute(Constants.attribDesc).Value = description;
            }

            RootElement.Save(FilePath);
        }

        public static void AddCornerToFile(string name, string description = "")
        {
            XElement xCorner = new XElement(ns + Constants.elemCorner, new XAttribute(Constants.attribName, name));

            if (description != "")
                xCorner.Add(new XAttribute(Constants.attribDesc, description));

            for (int d = 0; d < Constants.Directions.Length; d++)
            {
                XElement xEl2 = new XElement(ns + Constants.elemCount, new XAttribute(Constants.attribDir, Constants.Directions[d]));

                for (int m = 0; m < Constants.Modals.Length; m++)
                {
                    XElement xEl3 = new XElement(ns + Constants.elemValue, new XAttribute(Constants.attribModal, Constants.Modals[m]), 0);
                    xEl2.Add(xEl3);
                }

                xCorner.Add(xEl2);
            }

            xCorner.Add(new XElement(ns + Constants.elemPed,
                        new XElement(ns + Constants.elemValue, new XAttribute(Constants.attribCross, true), 0),
                        new XElement(ns + Constants.elemValue, new XAttribute(Constants.attribCross, false), 0)
                ));

            RootElement.Add(xCorner);
            RootElement.Save(FilePath);
        }

        #endregion

        #region CountItems

        public static List<CountItem> GetCountItemsFromFile(int cornerPosition)
        {
            List<CountItem> items;

            IEnumerable<XElement> values = 
                RootElement
                .Elements(ns + Constants.elemCorner).ElementAt(cornerPosition)
                .Elements(ns + Constants.elemCount)
                .Elements(ns + Constants.elemValue);

            items = (from v in values
                     orderby v.Parent.Elements().ToList().IndexOf(v)
                     select new CountItem(
                         (string)v.Parent.Attribute(Constants.attribDir),
                         (string)v.Attribute(Constants.attribModal),
                         int.Parse(v.Value)
                     )).ToList();

            values = RootElement
                .Elements(ns + Constants.elemCorner).ElementAt(cornerPosition)
                .Elements(ns + Constants.elemPed)
                .Elements(ns + Constants.elemValue);

            items.Add(new CountItem(true, int.Parse(values.ElementAt(0).Value)));
            items.Add(new CountItem(false, int.Parse(values.ElementAt(1).Value)));

            return items;
        }

        public static void SaveCountValuesInFile(int cornerPosition, List<CountItem> items)
        {
            XElement xCorner = RootElement.Elements(ns + Constants.elemCorner).ElementAt(cornerPosition);

            foreach (CountItem item in items)
            {
                if (!item.IsPed)
                {
                    XElement value = xCorner.Elements(ns + Constants.elemCount).Elements(ns + Constants.elemValue).Where((v) =>
                    {
                        return
                            (string)v.Parent.Attribute(Constants.attribDir) == item.Direction &&
                            (string)v.Attribute(Constants.attribModal) == item.Modal;
                    }).First();

                    value.Value = item.Value.ToString();
                }
            }

            xCorner.Element(ns + Constants.elemPed).Elements(ns + Constants.elemValue).ElementAt(0).Value = items[items.Count - 2].Value.ToString();
            xCorner.Element(ns + Constants.elemPed).Elements(ns + Constants.elemValue).ElementAt(1).Value = items[items.Count - 1].Value.ToString();

            RootElement.Save(FilePath);
        }

        #endregion
    }
}