using System;
using System.Text;

namespace Imfact.Coding.Builders
{
	public interface ICodeBuilder
	{
		void Append(string text);
		void AppendLine(string text = "");
		string GetText();

		/// <summary>
		/// デリゲートを介して文字列を構築することができます。
		/// デリゲートに渡される引数のオブジェクトから、特殊なフォーマットで文字列を構築する ICodeBuilder を取得できます。
		/// </summary>
		/// <param name="build"></param>
		/// <returns></returns>
		public static string OnPlainBuilder(Action<ICodeBuilder> build)
		{
			var builder = new PlainBuilder(new StringBuilder());
			build.Invoke(builder);
			return builder.GetText();
		}

		/// <summary>
		/// デリゲートを介して文字列を構築することができます。
		/// 文字列は1段階インデントされ、最初の行と最後の行にはブロックの開始記号と終了記号を配置します。
		/// </summary>
		/// <param name="build"></param>
		/// <param name="startToken"></param>
		/// <param name="endToken"></param>
		public void EnterBlock(Action<ICodeBuilder> build, string startToken = "{", string endToken = "}")
		{
			using var block = BlockBuilder.EnterBlock(this, startToken, endToken);
			build.Invoke(block);
		}

		/// <summary>
		/// デリゲートを介して文字列を構築することができます。
		/// AppendLineで文字列を追加するとき、行の区切りに影響のある他の ICodeBuilder の機能を無視します。
		/// </summary>
		/// <param name="build"></param>
		public void EnterChunk(Action<ICodeBuilder> build)
		{
			using var chunk = new ChunkBuilder(this);
			build.Invoke(chunk);
		}

		/// <summary>
		/// デリゲートを介して文字列を構築することができます。
		/// Append, AppendLine ごとに、文字列を指定した区切り記号で区切ります。
		/// </summary>
		/// <param name="build"></param>
		/// <param name="separator"></param>
		/// <param name="lineSeparator"></param>
		public void EnterCsv(Action<ICodeBuilder> build, string separator = ", ", string lineSeparator = ",")
		{
			var separated = new SeparatedBuilder(this)
			{
				Separator = separator,
				LineSeparator = lineSeparator
			};

			build.Invoke(separated);
		}

		/// <summary>
		/// デリゲートを介して文字列を構築することができます。
		/// AppendLine で複数行の文字列を生成するとき、すべての行のあいだに1行の行間を空けます。
		/// </summary>
		/// <param name="build"></param>
		public void EnterLineSpaced(Action<ICodeBuilder> build)
		{
			build.Invoke(new LineSpacedBuilder(this));
		}
	}
}