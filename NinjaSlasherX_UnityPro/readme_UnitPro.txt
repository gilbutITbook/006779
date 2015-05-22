=================================
Unity Pro高速化
=================================

Unity Proの機能を使って高速化したNinjaSlasherXのプロジェクトです。
Unity Proでビルドすると、下記の最適化が有効になります。
プログラム部分の高速化については、Unity Freeでも動作します。


---------------------------------
・UnityPROのスプライトのタイト化
・UnityPROのSprite Packerを利用したアトラス化と高速化
・UnityPROのStaticパッチングを利用した高速化

---------------------------------
・プログラムの高速化
　　・Physics2DのRayの高速化
	BaseCharacterController.cs
      　→接地判定のPhysics2D.OverlapPointAllをPhysics2D.OverlapPointNonAllocで高速化
	EnemyMain.cs
      　→カメラ外での接地判定のオフにする
　　・テキストの高速化
	PlayerController.cs

上記の高速化により、全体的に処理落ちがちょっと緩和されます。



