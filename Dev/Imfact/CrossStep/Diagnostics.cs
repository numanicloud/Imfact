using System;
using System.Collections.Generic;
using System.Text;

namespace Imfact.CrossStep;

// RegisterSourceOutput段階に入らないとReportDiagnosticを使えないので、
// ロガーをメソッドに仕込む方針は上手くなさそう。
// 生成されるノードに「エラー」というノードを持てるようにすべきかも
internal interface IDiagnostics
{
	void ReportInfo(string id, string title, string message);

	void ReportError(string id, string title, string message);
}