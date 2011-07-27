<%@ control language="C#" autoeventwireup="true" codebehind="List.ascx.cs" inherits="UmbracoLabs.Web.List" %>
<%@ import namespace="umbraco.NodeFactory" %>

<asp:literal id="litHeader" runat="server"/>

<asp:listview id="lisItems" enableviewstate="false" onitemdatabound="lisItems_ItemDataBound" onitemcommand="lisItems_ItemCommand" runat="server">
<itemtemplate>
	<div class="item">
		<h4><%#Eval("Name")%></h4>
		<p><%#(Container.DataItem as Node).GetProperty("summaryText").Value%></p>
	</div>
</itemtemplate>
<emptyitemtemplate>
	<div class="empty">No records found.</div>
</emptyitemtemplate>
</asp:listview>

<%--
References:
http://billrob.com/archive/2007/02/10/How-to-add-automatically-add-namespaces-to-aspx-pages.aspx
http://weblogs.asp.net/scottgu/archive/2007/08/10/the-asp-listview-control-part-1-building-a-product-listing-page-with-clean-css-ui.aspx
http://tuvian.wordpress.com/2011/06/14/how-to-use-listview-control-in-asp-net/
--%>

<asp:label id="lblError" cssclass="error" runat="server"/>
<asp:label id="lblLog" cssclass="log" runat="server"/>