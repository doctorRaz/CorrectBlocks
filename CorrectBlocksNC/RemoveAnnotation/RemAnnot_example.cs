using HostMgd.ApplicationServices;
using HostMgd.EditorInput;

using Teigha.DatabaseServices;
using Teigha.LayerManager;
using Teigha.Runtime;

using App = HostMgd.ApplicationServices;
using Db = Teigha.DatabaseServices;
using System.Collections.Generic;
using System.Linq;
using HostMgd.Runtime;


// идея https://adn-cis.org/forum/index.php?topic=10382.msg48321#msg48321
//ResultBuffer 

namespace BD_Function
{
    public class LispFunctionsExtensions
    {
        [LispFunction(nameof(RemoveAnnotation))]
        public bool RemoveAnnotation(ResultBuffer resBuf)
        {
            return SetAnnotation(resBuf, false);
        }
 
        [LispFunction(nameof(AddAnnotation))]
        public static bool AddAnnotation(ResultBuffer resBuf)
        {
            return SetAnnotation(resBuf, true);
        }
 
        private static bool SetAnnotation(ResultBuffer resBuf, bool add)
        {
            if (resBuf == null)
            {
                return false;
            }

            TypedValue[] argArray = resBuf.AsArray();
            TypedValue typedValue = argArray[0];
 
            if (typedValue.TypeCode != (short)LispDataType.ObjectId)
            {
                return false;
            }

            ObjectId annObjId = (ObjectId)typedValue.Value;
 
            if (IsObjectAnnotation(annObjId) == false)
            {
                return false;
            }
 
            using (var tr = annObjId.Database.TransactionManager.StartTransaction())
            {
                DBObject obj = tr.GetObject(annObjId, OpenMode.ForWrite, false, true);
                obj.Annotative = add ? AnnotativeStates.True : AnnotativeStates.False;
 
                if (add)
                {
                    AddCurrentScale(obj);
                }
 
                tr.Commit();
            }
            if (IsObjectBlockTableRecord(annObjId) && add)
            {
                AddCurrentScaleToBlockRef(annObjId);
            }
 
            return true;
        }
 
        private static bool IsObjectBlockTableRecord(ObjectId annObjId)
        {
            return annObjId.ObjectClass.Equals(RXClass.GetClass(typeof(BlockTableRecord)));
        }
 
        private static void AddCurrentScale(DBObject dbObj)
        {
            if (!IsDbObjectBlockTableRecord(dbObj))            
            {
                Database db = Application.DocumentManager.MdiActiveDocument.Database;
                ObjectContextManager ocm = db.ObjectContextManager;
                ObjectContextCollection occ = ocm.GetContextCollection("ACDB_ANNOTATIONSCALES");
                string cannoscale = Application.GetSystemVariable("CANNOSCALE") as string;
                dbObj.AddContext(occ.GetContext(cannoscale));
            }
        }
 
        private static void AddCurrentScaleToBlockRef(ObjectId annObjId)
        {
            using (var tr = annObjId.Database.TransactionManager.StartTransaction())
            {
                BlockTableRecord blockTableRecord = (BlockTableRecord)tr.GetObject(annObjId, OpenMode.ForRead, false, true);
               
                // получаем все вставки  блока
                ObjectIdCollection blockRefs = blockTableRecord.GetBlockReferenceIds(true, true);
 
                // если блок динамический
                if (blockTableRecord.IsDynamicBlock)
                {
                    // получаем все анонимные блоки динамического блока
                    ObjectIdCollection anonymousIds = blockTableRecord.GetAnonymousBlockIds();
                    foreach (ObjectId anonymousBtrId in anonymousIds)
                    {
                        // получаем анонимный блок
                        BlockTableRecord anonymousBtr = (BlockTableRecord)tr.GetObject(anonymousBtrId, OpenMode.ForRead);
                        // получаем все вставки этого блока
                        ObjectIdCollection blockRefIds = anonymousBtr.GetBlockReferenceIds(true, true);
                        foreach (ObjectId id in blockRefIds)
                        {
                            blockRefs.Add(id);
                        }
                    }
                }
                foreach (ObjectId objectId in blockRefs)
                {
                    var obj = tr.GetObject(objectId, OpenMode.ForWrite, false, true);
                    AddCurrentScale(obj);
                }
                tr.Commit();
            }
        }
 
        private static readonly List<RXClass> AnnotativeClasses = new[]
        {
        typeof(DBText),
        typeof(BlockTableRecord),
        typeof(MText),
        typeof(Dimension),
        typeof(Leader),
        typeof(MLeader),
        typeof(Hatch)
         }
        .Select(RXObject.GetClass)
        .ToList();
 
        private static bool IsObjectAnnotation(ObjectId annObjId)
        {
            return AnnotativeClasses.Any(t => annObjId.ObjectClass.Equals(t));
        }
 
        private static bool IsDbObjectBlockTableRecord(DBObject dBObject)
        {
            return dBObject.GetRXClass().Equals(RXClass.GetClass(typeof(BlockTableRecord)));
        }
 
    }
}