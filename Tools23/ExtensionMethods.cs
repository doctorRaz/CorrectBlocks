using System;
using System.Collections.Generic;

#if NC
using Teigha.DatabaseServices;
using Teigha.Runtime;

using AcRx = Teigha.Runtime;
#else

using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using AcRx = Autodesk.AutoCAD.Runtime;
#endif
/* от Gilles Chanteau
 * http://www.theswamp.org/index.php?topic=31859.msg508802#msg508802
 * Hi,
This is a simple and 'brute force' method to mimic the ATTSYNC command. It replaces the existing AttributeReferences from the inserted BlockReferences with new ones from the BlockTableRecord AttributeDefinitions.
The SynchronizeAttributes() method is defined as an extension method for the BlockTableRecord type so that it can be called as an instance method of this type.
Using example (assuming btr is a BlocTableRecord instance) : btr.SynchronizeAttributes()
<EDIT: corrected an issue w/ attributes in dynamic blocks>
<EDIT: corrected an issue w/ constant attributes>
*------------------
*Привет,

Это простой и "грубой силы" метод, чтобы имитировать команду ATTSYNC. Он заменяет существующие AttributeReferences из вставленных BlockReferences на новые из BlockTableRecord AttributeDefinitions.
Метод SynchronizeAttributes() определяется как метод расширения для типа BlockTableRecord, чтобы его можно было вызвать как метод экземпляра этого типа.
SynchronizeAttributes()

<EDIT: corrected an issue w/ attributes in dynamic blocks><EDIT: corrected an issue w/ attributes in dynamic blocks>
<EDIT: corrected an issue w/ constant attributes>
*
 */
namespace GillesChanteau
{
    /// <summary>
    /// Это простой и "грубой силы" метод, чтобы имитировать команду ATTSYNC. Он заменяет существующие AttributeReferences из вставленных BlockReferences на новые из BlockTableRecord AttributeDefinitions.
    ///Метод SynchronizeAttributes() определяется как метод расширения для типа BlockTableRecord, чтобы его можно было вызвать как метод экземпляра этого типа.
    ///SynchronizeAttributes()
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// 
        /// </summary>
        static RXClass attDefClass = RXClass.GetClass(typeof(AttributeDefinition));

        public static void SynchronizeAttributes(this BlockTableRecord target)
        {
            if (target == null)
                throw new ArgumentNullException("target");

            Transaction tr = target.Database.TransactionManager.TopTransaction;
            if (tr == null)
                throw new AcRx.Exception(ErrorStatus.NoActiveTransactions);
            List<AttributeDefinition> attDefs = target.GetAttributes(tr);
            foreach (ObjectId id in target.GetBlockReferenceIds(true, false))
            {
                BlockReference br = (BlockReference)tr.GetObject(id, OpenMode.ForWrite);
                br.ResetAttributes(attDefs, tr);
            }
            if (target.IsDynamicBlock)
            {
                target.UpdateAnonymousBlocks();
                foreach (ObjectId id in target.GetAnonymousBlockIds())
                {
                    BlockTableRecord btr = (BlockTableRecord)tr.GetObject(id, OpenMode.ForRead);
                    attDefs = btr.GetAttributes(tr);
                    foreach (ObjectId brId in btr.GetBlockReferenceIds(true, false))
                    {
                        BlockReference br = (BlockReference)tr.GetObject(brId, OpenMode.ForWrite);
                        br.ResetAttributes(attDefs, tr);
                    }
                }
            }
        }

        private static List<AttributeDefinition> GetAttributes(this BlockTableRecord target, Transaction tr)
        {
            List<AttributeDefinition> attDefs = new List<AttributeDefinition>();
            foreach (ObjectId id in target)
            {
                if (id.ObjectClass == attDefClass)
                {
                    AttributeDefinition attDef = (AttributeDefinition)tr.GetObject(id, OpenMode.ForRead);
                    attDefs.Add(attDef);
                }
            }
            return attDefs;
        }

        private static void ResetAttributes(this BlockReference br, List<AttributeDefinition> attDefs, Transaction tr)
        {
            Dictionary<string, string> attValues = new Dictionary<string, string>();
            foreach (ObjectId id in br.AttributeCollection)
            {
                if (!id.IsErased)
                {
                    AttributeReference attRef = (AttributeReference)tr.GetObject(id, OpenMode.ForWrite);
                    attValues.Add(attRef.Tag,
                        attRef.IsMTextAttribute ? attRef.MTextAttribute.Contents : attRef.TextString);
                    attRef.Erase();
                }
            }
            foreach (AttributeDefinition attDef in attDefs)
            {
                AttributeReference attRef = new AttributeReference();
                attRef.SetAttributeFromBlock(attDef, br.BlockTransform);
                if (attDef.Constant)
                {
                    attRef.TextString = attDef.IsMTextAttributeDefinition ?
                        attDef.MTextAttributeDefinition.Contents :
                        attDef.TextString;
                }
                else if (attValues.ContainsKey(attRef.Tag))
                {
                    attRef.TextString = attValues[attRef.Tag];
                }
                br.AttributeCollection.AppendAttribute(attRef);
                tr.AddNewlyCreatedDBObject(attRef, true);
            }
        }
    }
}