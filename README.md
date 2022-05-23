# 專案目標  
目標是模擬 ASP.NET MVC 的程式碼產生器(Scaffold)，  
也就是針對已經建立好的類別，來快速產生客製化的 Controller 和 View 模板，  
預設產生出來的模板只適合套用在我的個人環境，若有需要可自行改寫。  
&emsp;  
# 使用前提  
假設已經開好DB資料表，並且已經設定好 EFCore 用來連接 DB 的 context  
那麼你應該可以取得下列參數  
1.專案名稱  
2.context名稱  
3.目標類別的表名  
4.目標類別的絕對路徑  
&emsp;  
# 使用教學  
1.將程式中的 D:/Desktop 修改成你想要的輸出路徑  
2.編譯並運行程式後，依序輸入對應的參數(以逗號區隔)，即可在輸出路徑找到產出的程式碼  
&emsp;  
# 開發環境  
Win10(家用版) + Visual Studio 2022 + .NET Core 3.1 (主控台應用程式)  
&emsp;