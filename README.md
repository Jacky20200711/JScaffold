# 專案目標  
針對已經建立好的類別(通常是 Models 資料夾底下的某個類別)，  
來模擬 Scaffold 快速產生 Controller 和 View 的程式碼模板，  
其中 View 的模板請根據自己常用的版面做調整。  
&emsp;  
# 使用前提  
假設已經開好DB資料表，並且已經設定好 EFCore 用來連接 DB 的 context  
那麼你應該可以取得下列五項參數  
1.專案名稱  
2.類別名稱  
3.context名稱  
4.此類別在context的表名  
5.此類別的絕對路徑  
&emsp;  
# 使用教學  
1.將程式中的 D:/Desktop 修改成你想要的輸出路徑  
2.編譯並運行程式後，依序輸入對應的參數(以逗號區隔)，即可產出對應的程式碼  
&emsp;  
# 開發環境  
Win10(家用版) + Visual Studio 2022 + .NET Core 3.1 (主控台應用程式)  
&emsp;