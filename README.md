# README #

#### これはなに？

Windows専用　ウイルスゲノム型判定ソフトウエア

FluGASv2 -Influenza Genome Assembly and subtyping-

FluGAS ver2.5 リリース版です。





#### どんなことに使うの？

高速シーケンサから出力されたインフルエンザウイルスゲノムFastqからウイルスの型を判定するソフトです。

おもに鳥インフルエンザのデータを使ってテストを行っています。

他のインフルエンザ ウイルスの型判定も行えます。

GISAIDが提供している登録用マクロを含むエクセルファイルを同梱しており、解析結果のコンセンサス塩基配列をエクセルファイルに反映する機能があります

（そのほかの必要事項は解析者が入力する必要があります）。

  

  

#### 使い方。

ダウンロードページの［Download］ボタンからZipファイルをダウンロードします。

https://github.com/FluGAS-dev/FluGASv25/blob/main/FluGAS.zip

（145MB）

  

Zipファイルを解凍するとマニュアルとインストーラ（*FluGASv2setup.exe*）が含まれています。インストーラを実行するとインストールが始まります。インストール先に日本語やスペースを含まないディレクトリを指定します。

```
EX) 
　　C:\FluGAS
　　C:\USERS￥［ユーザー名］\FluGAS 
```

インストール後に含まれている［**FluGASv25.exe**］をダブルクリックするか、スタートに登録されているFluGASv25 から起動します。

詳しい使い方は起動後のヘルプにからManualを参照するか、解凍後に含まれるdata\フォルダいかにあるPDFを参照してください。



起動時にRuntimeがないというエラーが出た場合はMicrosoft社が提供するRuntimeのインストールが必要になります。

###### ダウンロードページ

https://dotnet.microsoft.com/download/dotnet-core/3.1

###### .NET Core Desktop Runtime 3.1.11 　(2021.01.20 現在)

https://dotnet.microsoft.com/download/dotnet-core/thank-you/runtime-desktop-3.1.11-windows-x64-installer

  

  

#### 用意するもの。

Illumina 高速シーケンサ（Miseq、HiSeq など）、Nanopre （MinION、GridIONなど）でインフルエンザ ウイルスゲノムをシーケンスした Fast5 またはFastqファイル

（Nanopore Fast5 ファイルの場合、別途Nanopore社が提供するGuppyが必要）

  

  

#### 解析フロー。

シーケンスデータ　＝＞　NCBIリファレンスへマッピング（1回目）

マッピング結果（BAM）＝＞ 変異検出（samtools pileup）

**※ 変異検出を反映させたコンセンサス塩基配列作成（1回目）**

コンセンサス配列 ＝＞　NCBIリファレンスへBLAST（TopHitリファレンス取得）

シーケンスデータ　＝＞　BLAST結果TopHitリファレンス1本に対してへマッピング（2回目）

マッピング結果（BAM）＝＞ 変異検出（samtools pileup）

**※ 変異検出を反映させたコンセンサス塩基配列作成（2回目）**

  

  

#### 特徴。

多くのリファレンス配列にマッピングを行う場合、リードが酷似した塩基配列を含む複数のリファレンスにマッピングされます。

マッピングツールのゆらぎにより偏ったマッピング結果になることを回避するため、マッピング結果から作成したコンセンサス配列を、BLAST結果のリファレンスに対しすべてのリードを個別マッピングを行うことにより、ツールによるゆらぎを解消することを目的にフローを作成しました。

個別にコマンドを実行すると、個々のコマンド結果を待ち次のコマンドを実行することになり、さらに各分節別を行うため時間を要することを解消します。

データの指定とボタン1つで実行が可能なため、研究者・実験者以外のオペレータが実行可能となります。



#### 問い合わせ

株）ワールド・フュージョン  https://www.w-fusion.co.jp/

お問い合わせページ　https://www.w-fusion.co.jp/contact

メール techsupport［at］w-fusion.co.jp 

