/**
@file
    RenderRazor.cs
@version
    0.1
@date
    - Created: 2011-08-19
    - Modified: 2011-08-19
    .
@note
    References:
    - General:
        - http://our.umbraco.org/wiki/reference/code-snippets/razor-snippets/render-razor-scripts-for-emails-and-more
        .
    .
*/
using System;
using System.Collections.Generic;
using umbraco.cms.businesslogic.macro;
using umbraco.MacroEngines;

namespace UmbracoLabs {

/// <summary>Render razor script to System.String.</summary>
public class RenderRazor
{
    public static string Render(string razorScript = "", string macroScriptFileName = "", int nodeId = 0, IDictionary<string, string> macroParameters = null)
    {
        var macroEngine = new RazorMacroEngine();
        var macro = new MacroModel();

        macro.ScriptCode = razorScript;
        macro.ScriptLanguage = "cshtml";
        macro.ScriptName = macroScriptFileName;

        var node = new umbraco.NodeFactory.Node(nodeId);

        if(macroParameters != null) {
            foreach(var param in macroParameters) {
                macro.Properties.Add(new MacroPropertyModel(param.Key, param.Value));
            }
        }
        return macroEngine.Execute(macro, new umbraco.NodeFactory.Node(nodeId));
    }

    public static string RenderRazorScriptString(string razorScript, int nodeId, IDictionary<string, string> macroParameters = null)
    {
        return Render(razorScript, String.Empty, nodeId, macroParameters);
    }

    public static string RenderRazorScriptFile(string macroScriptFileName, int nodeId, IDictionary<string, string> macroParameters = null)
    {
        return Render(String.Empty, macroScriptFileName, nodeId, macroParameters);
    }

}

} // END namespace UmbracoLabs