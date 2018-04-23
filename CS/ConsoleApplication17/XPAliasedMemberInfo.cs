using System;
using DevExpress.Data.Filtering;
using DevExpress.Data.Filtering.Helpers;

namespace DevExpress.Xpo.Metadata {
    public class XPAliasedMemberInfo : XPMemberInfo {
        readonly string propertyName;
        readonly Type propertyType;
        public XPAliasedMemberInfo(XPClassInfo owner, string propertyName, Type propertyType, string expression)
            : base(owner, true) {
            if (propertyName == null)
                throw new ArgumentNullException("propertyName");
            this.propertyName = propertyName;
            this.propertyType = propertyType;
            Owner.AddMember(this);
            this.AddAttribute(new PersistentAliasAttribute(expression));
        }
        public override string Name { get { return propertyName; } }
        public override bool IsPublic { get { return true; } }
        public override Type MemberType { get { return propertyType; } }
        protected override bool CanPersist { get { return false; } }
        public override object GetValue(object theObject) {
            bool caseSensitive = XpoDefault.DefaultCaseSensitive;
            if (theObject is DevExpress.Xpo.Helpers.ISessionProvider) {
                caseSensitive = ((DevExpress.Xpo.Helpers.ISessionProvider)theObject).Session.CaseSensitive;
            }
            PersistentAliasAttribute persistentAliasAttribute = (PersistentAliasAttribute)this.GetAttributeInfo(typeof(PersistentAliasAttribute));
            return new ExpressionEvaluator(Owner.GetEvaluatorContextDescriptor(), CriteriaOperator.Parse(persistentAliasAttribute.AliasExpression, new object[0]),
                caseSensitive, Owner.Dictionary.CustomFunctionOperators).Evaluate(theObject);
        }
        public override void SetValue(object theObject, object theValue) { }
    }

    #region .NET Framework 3+ Extender
    public static class XPAliasedMemberInfoExtender {
        public static XPAliasedMemberInfo CreateAliasedMember(this XPClassInfo self, string name, Type type, string expression) {
            return XPAliasedMemberInfoExtender.CreateAliasedMember(self, name, type, expression, null);
        }
        public static XPAliasedMemberInfo CreateAliasedMember(this XPClassInfo self, string name, Type type, string expression, Attribute[] attrs) {
            XPAliasedMemberInfo result = new XPAliasedMemberInfo(self, name, type, expression);
            if (attrs != null) {
                foreach (Attribute a in attrs) {
                    result.AddAttribute(a);
                }
            }
            return result;
        }
    }
    #endregion
}
