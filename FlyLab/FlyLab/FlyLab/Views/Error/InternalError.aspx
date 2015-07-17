﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Nested.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Body" runat="server">

    <h2 id="errorcode">500 Internal Server Error</h2>
    <div id="errordiv">
        <img id="errorimg" src="../Assets/Imgs/500internal.gif" alt="Error 500 Image" title="You should probably get that checked out." />
    </div>
    <h4>Something broke. Sorry about that.</h4>

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
