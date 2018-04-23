using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using DevExpress.Data.Filtering;

namespace ConsoleApplication17 {
    class Program {
        static void Main(string[] args) {

            XpoDefault.DataLayer = new SimpleDataLayer(new DevExpress.Xpo.DB.InMemoryDataStore());
            XpoDefault.Session = null;

            using (UnitOfWork uow = new UnitOfWork()) {
                uow.ClearDatabase();
                TestClass c1 = new TestClass(uow);
                c1.Name = "aaa"; c1.Email = "e@mail.com";
                TestClass c2 = new TestClass(uow);
                c2.Name = "bbb"; c2.Email = "nobody@w3.com";
                c2.Master = c1;
                uow.CommitChanges();
            }

            XPClassInfo ci = XpoDefault.DataLayer.Dictionary.GetClassInfo(typeof(TestClass));
            ci.CreateAliasedMember("DisplayName", typeof(string), "concat([Name],' (',[Email],')',iif([Master] is null,'',Concat(' managed by ',[Master].Name)))");

            using (UnitOfWork uow = new UnitOfWork()) {
                XPCollection<TestClass> xpc = new XPCollection<TestClass>(uow, CriteriaOperator.Parse("Contains([DisplayName],'w3')"));
                System.Diagnostics.Debug.Assert(xpc.Count == 1);
                System.Diagnostics.Debug.Assert(xpc[0].Name == "bbb");
                XPMemberInfo mi = xpc[0].ClassInfo.FindMember("DisplayName");
                System.Diagnostics.Debug.Assert(mi!= null && mi.IsReadOnly && mi.IsAliased);
                System.Diagnostics.Debug.Assert(object.Equals(mi.GetValue(xpc[0]), "bbb (nobody@w3.com) managed by aaa"));
            }
        }
    }

    public class TestClass : XPObject {
        public TestClass(Session s) : base(s) { }

        public string Name {
            get { return GetPropertyValue<string>("Name"); }
            set { SetPropertyValue<string>("Name", value); }
        }
        public string Email {
            get { return GetPropertyValue<string>("Email"); }
            set { SetPropertyValue<string>("Email", value); }
        }
        public TestClass Master {
            get { return GetPropertyValue<TestClass>("Master"); }
            set { SetPropertyValue<TestClass>("Master", value); }
        }
    }
}
