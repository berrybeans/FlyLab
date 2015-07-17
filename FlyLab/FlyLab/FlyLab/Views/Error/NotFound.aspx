<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Nested.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Body" runat="server">

    <h2 id="errorcode">404 Not Found</h2><br />
    <div id="errordiv">
        <img id="errorimg" src="../Assets/Imgs/404notfound.gif" alt="Error 404 Image" title="No, you can't actually type in this console."/>
    </div>
    <h4>The page you requested has been moved, doesn't exist, or was eaten by a Grue.</h4>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="head" runat="server">
    <style>
        h4 {
            text-align: center;
        }
        #errorcode {
            text-align: center;
        }
        #errordiv {
            position: relative;
            margin-left: auto;
            margin-right: auto;
            padding-bottom: 2em;
        }
        #errorimg {
            display: block;
            margin: auto;
        }
    </style>
</asp:Content>
