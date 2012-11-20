<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Flannel.aspx.cs" Inherits="Flannel__Web_App_.Flannel" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Flannel</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
    </div>
        <h1>
            Artists:
        </h1>
        <asp:RadioButtonList ID="RadioButtonList1" runat="server" OnSelectedIndexChanged="RadioButtonList1_Click">
        </asp:RadioButtonList>
        <asp:Label ID="Label1" runat="server" Text=""></asp:Label>
    </form>
</body>
</html>
