using System.Linq;
using System.Reflection;
using Abp.Utils.Extensions;

namespace Abp.WebApi.Controllers.Dynamic.Scripting.Actions
{
    internal abstract class ActionScriptProxyGenerator
    {
        protected DynamicApiControllerInfo ControllerInfo { get; private set; }

        protected DynamicApiActionInfo ActionInfo { get; private set; }

        private const string JsMethodTemplate =
@"    var {jsMethodName} = function({jsMethodParameterList}) {
        return abp.ajax($.extend({
{ajaxCallParameters}
        }, ajaxParams));
    };";

        protected ActionScriptProxyGenerator(DynamicApiControllerInfo controllerInfo, DynamicApiActionInfo methodInfo)
        {
            ControllerInfo = controllerInfo;
            ActionInfo = methodInfo;
        }

        public virtual string GenerateMethod()
        {
            var jsMethodName = ActionInfo.ActionName.ToCamelCase();
            var jsMethodParameterList = GenerateJsMethodParameterList(ActionInfo.Method);

            var jsMethod = JsMethodTemplate
                .Replace("{jsMethodName}", jsMethodName)
                .Replace("{jsMethodParameterList}", jsMethodParameterList)
                .Replace("{ajaxCallParameters}", GenerateAjaxCallParameters());

            return jsMethod;
        }

        protected abstract string GenerateAjaxCallParameters();

        protected virtual string PlainActionUrl
        {
            get
            {
                return "/api/services/" + ControllerInfo.Name.ToCamelCase() + "/" + ActionInfo.ActionName.ToCamelCase();
            }
        }

        protected string GenerateJsMethodParameterList(MethodInfo methodInfo)
        {
            var paramNames = methodInfo.GetParameters().Select(prm => prm.Name.ToCamelCase()).ToList();
            paramNames.Add("ajaxParams");
            return string.Join(", ", paramNames);
        }
    }
}