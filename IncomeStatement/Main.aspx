<%@ Page Language="C#" AutoEventWireup="true" Inherits="Main" Codebehind="Main.aspx.cs" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
    <head runat="server">
        <title>IncomeStatement</title>
        <meta http-equiv="Pragma" content="no-cache" />
        <meta http-equiv="Cache-Control" content="no-cache, must-revalidate"/>
        <meta http-equiv="expires" content="0" />
        <meta name="viewport" content="width=device-width, initial-scale=1, shrink-to-fit=no" />

        <!--import JS and CSS file -->

        <script  type="text/javascript" src="/IncomeStatement/WebData/BundleResult/Main/IS_20191029_min_v1.js"></script>
        <link rel="stylesheet" href="/IncomeStatement/WebData/BundleResult/Main/IS_20191029_min_v1_CSS.css" />
<!--
        <script  type="text/javascript" src="http://localhost:8080/JsFile/Internal/Main/IS_20191029_min_v1.js"></script>
        <link rel="stylesheet" href="http://localhost:8080/JsFile/Internal/Main/IS_20191029_min_v1_CSS.css" />
-->
    </head>
    <body>
        <div id="vue-instance">
            <div id="leftMenu">
                <p id="title">
                    使用者ID：{{userInfo.id}} <br />
                    使用者名稱：{{userInfo.name}}<br />
                    角色：{{userInfo.role}}<br />
                </p>
                <hr />
                <template v-for="item in menu">
                    <div class="menuItem button" squared block @click="item.isShowChild = !item.isShowChild">
                        <span style="color: white">{{item.text}}</span>
                        <img :src="$options.imgSrc.dropDown" width="32px" class="dropIcon">
                    </div>

                    <b-collapse v-model="item.isShowChild" :id="`collaspe_${item.key}`">
                        <ul class="submenuUL" v-if="item.key === `reportPage`">
                            <li
                                v-for="submenu in supportSubMenu(item.key)"
                                class="button"
                                @click="openReport(submenu.key)"
                            >{{submenu.text}}</li>
                        </ul>
                        <ul class="submenuUL" v-else>
                            <li
                                v-for="submenu in supportSubMenu(item.key)"
                                class="button"
                                @click="currentPageKey = submenu.key"
                            >{{submenu.text}}</li>
                        </ul>
                    </b-collapse>
                </template>
                <hr />
                <div id="btnLogout" class="button" @click="logout">登出</div>
            </div>
            <div id="rightContent">
                <p id="helloMessage" v-if="currentPageKey.length === 0">
                    您好，歡迎使用<b>家庭收支記帳調查編碼系統</b><br />請點選功能操作！
                </p>
                <component :is="currentPageKey" v-if="supportedComponent.includes(currentPageKey)"></component>
                <p v-else>Not supported</p>
            </div>
        </div>
    </body>
</html>

<style scoped>
#leftMenu {
    position: relative;
    width: 16%;
    background-color: #61C7D0;
    color: #000;
    overflow-y: auto;
    border-radius: 1vw;
    margin:32px 0 16px 16px;
    display: inline-block;
    float: left;
}
#title {
    font-size: 14px;
    text-align: center;
    padding: 11px 8px 0 8px;
}
.menuItem {
    font-size: 0.9rem;
    width: 100%;
    text-align: center;
    padding: 8px;
    background-color: #138496;
}
.menuItem:hover {
    background-color: #93D2D8;
}
.dropIcon {
    position: absolute;
    right: 8px;
}
.submenuUL {
    font-size: 17px;
    text-align: left;
    padding-left: 8%;
    list-style: none;
}
.submenuUL li {
    padding: 8px;
}
.submenuUL li:hover{
    background-color: #93D2D8;
}
#btnLogout {
    bottom: 8%;
    padding: 8px;
    background-color: #6E7E85;
    text-align: center;
    width: 100%;
}
#btnLogout:hover {
    background-color: #6EA7AC;
}
#rightContent {
    position: relative;
    display: inline-block;
    float: left;
    width: 80%;
    min-height: 500px;
    border-radius: 1vw;
    margin: 16px 0 16px 16px;
}
#helloMessage {
    text-align: center;
    margin-top: 60px;
    color: #137C8D;
}
</style>