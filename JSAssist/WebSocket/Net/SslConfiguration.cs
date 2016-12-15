using System;
using System.Net.Security;
using System.Runtime.CompilerServices;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

namespace WebSocket.Net
{
	public abstract class SslConfiguration
	{
		private LocalCertificateSelectionCallback _certSelectionCallback;

		private RemoteCertificateValidationCallback _certValidationCallback;

		private bool _checkCertRevocation;

		private SslProtocols _enabledProtocols;

		protected LocalCertificateSelectionCallback CertificateSelectionCallback
		{
			get
			{
				// 
				// Current member / type: System.Net.Security.LocalCertificateSelectionCallback WebSocket.Net.SslConfiguration::get_CertificateSelectionCallback()
				// File path: C:\Users\Ryu\Desktop\방송\JSAssist\JSAssist.exe
				// 
				// Product version: 2016.3.1003.0
				// Exception in: System.Net.Security.LocalCertificateSelectionCallback get_CertificateSelectionCallback()
				// 
				// 동일한 키를 사용하는 항목이 이미 추가되었습니다.
				//    위치: System.ThrowHelper.ThrowArgumentException(ExceptionResource resource)
				//    위치: System.Collections.Generic.Dictionary`2.Insert(TKey key, TValue value, Boolean add)
				//    위치: System.Collections.Generic.Dictionary`2.System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<TKey,TValue>>.Add(KeyValuePair`2 keyValuePair)
				//    위치: ..AddRange[,](IDictionary`2 , IDictionary`2 ) 파일 c:\Builds\556\Behemoth\ReleaseBranch Production Build NT\Sources\OpenSource\Cecil.Decompiler\Common\Extensions.cs:줄 99
				//    위치: ..( ) 파일 c:\Builds\556\Behemoth\ReleaseBranch Production Build NT\Sources\OpenSource\Cecil.Decompiler\Decompiler\MethodSpecificContext.cs:줄 181
				//    위치: ..( ) 파일 c:\Builds\556\Behemoth\ReleaseBranch Production Build NT\Sources\OpenSource\Cecil.Decompiler\Steps\RebuildLambdaExpressions.cs:줄 66
				//    위치: ..( ) 파일 c:\Builds\556\Behemoth\ReleaseBranch Production Build NT\Sources\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:줄 89
				//    위치: ..Visit( ) 파일 c:\Builds\556\Behemoth\ReleaseBranch Production Build NT\Sources\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:줄 270
				//    위치: ..Visit[,]( ) 파일 c:\Builds\556\Behemoth\ReleaseBranch Production Build NT\Sources\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:줄 280
				//    위치: ..Visit( ) 파일 c:\Builds\556\Behemoth\ReleaseBranch Production Build NT\Sources\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:줄 316
				//    위치: ..( ) 파일 c:\Builds\556\Behemoth\ReleaseBranch Production Build NT\Sources\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:줄 604
				//    위치: ..( ) 파일 c:\Builds\556\Behemoth\ReleaseBranch Production Build NT\Sources\OpenSource\Cecil.Decompiler\Steps\RebuildLambdaExpressions.cs:줄 118
				//    위치: ..( ) 파일 c:\Builds\556\Behemoth\ReleaseBranch Production Build NT\Sources\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:줄 125
				//    위치: ..Visit( ) 파일 c:\Builds\556\Behemoth\ReleaseBranch Production Build NT\Sources\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:줄 270
				//    위치: ..( ) 파일 c:\Builds\556\Behemoth\ReleaseBranch Production Build NT\Sources\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:줄 523
				//    위치: ..( ) 파일 c:\Builds\556\Behemoth\ReleaseBranch Production Build NT\Sources\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:줄 97
				//    위치: ..Visit( ) 파일 c:\Builds\556\Behemoth\ReleaseBranch Production Build NT\Sources\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:줄 270
				//    위치: ..( ) 파일 c:\Builds\556\Behemoth\ReleaseBranch Production Build NT\Sources\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:줄 377
				//    위치: ..( ) 파일 c:\Builds\556\Behemoth\ReleaseBranch Production Build NT\Sources\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:줄 59
				//    위치: ..Visit( ) 파일 c:\Builds\556\Behemoth\ReleaseBranch Production Build NT\Sources\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:줄 270
				//    위치: ..Visit[,]( ) 파일 c:\Builds\556\Behemoth\ReleaseBranch Production Build NT\Sources\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:줄 280
				//    위치: ..Visit( ) 파일 c:\Builds\556\Behemoth\ReleaseBranch Production Build NT\Sources\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:줄 311
				//    위치: ..( ) 파일 c:\Builds\556\Behemoth\ReleaseBranch Production Build NT\Sources\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:줄 331
				//    위치: ..( ) 파일 c:\Builds\556\Behemoth\ReleaseBranch Production Build NT\Sources\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:줄 49
				//    위치: ..Visit( ) 파일 c:\Builds\556\Behemoth\ReleaseBranch Production Build NT\Sources\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:줄 270
				//    위치: ..( ) 파일 c:\Builds\556\Behemoth\ReleaseBranch Production Build NT\Sources\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:줄 355
				//    위치: ..( ) 파일 c:\Builds\556\Behemoth\ReleaseBranch Production Build NT\Sources\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:줄 55
				//    위치: ..Visit( ) 파일 c:\Builds\556\Behemoth\ReleaseBranch Production Build NT\Sources\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:줄 270
				//    위치: ..Visit[,]( ) 파일 c:\Builds\556\Behemoth\ReleaseBranch Production Build NT\Sources\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:줄 280
				//    위치: ..Visit( ) 파일 c:\Builds\556\Behemoth\ReleaseBranch Production Build NT\Sources\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:줄 311
				//    위치: ..( ) 파일 c:\Builds\556\Behemoth\ReleaseBranch Production Build NT\Sources\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:줄 331
				//    위치: ..( ,  ) 파일 c:\Builds\556\Behemoth\ReleaseBranch Production Build NT\Sources\OpenSource\Cecil.Decompiler\Steps\RebuildLambdaExpressions.cs:줄 130
				//    위치: ..(MethodBody ,  , ILanguage ) 파일 c:\Builds\556\Behemoth\ReleaseBranch Production Build NT\Sources\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:줄 88
				//    위치: ..(MethodBody , ILanguage ) 파일 c:\Builds\556\Behemoth\ReleaseBranch Production Build NT\Sources\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:줄 70
				//    위치: ..(MethodBody , & ) 파일 c:\Builds\556\Behemoth\ReleaseBranch Production Build NT\Sources\OpenSource\Cecil.Decompiler\Decompiler\PropertyDecompiler.cs:줄 345
				// 
				// mailto: JustDecompilePublicFeedback@telerik.com

			}
			set
			{
				this._certSelectionCallback = value;
			}
		}

		protected RemoteCertificateValidationCallback CertificateValidationCallback
		{
			get
			{
				// 
				// Current member / type: System.Net.Security.RemoteCertificateValidationCallback WebSocket.Net.SslConfiguration::get_CertificateValidationCallback()
				// File path: C:\Users\Ryu\Desktop\방송\JSAssist\JSAssist.exe
				// 
				// Product version: 2016.3.1003.0
				// Exception in: System.Net.Security.RemoteCertificateValidationCallback get_CertificateValidationCallback()
				// 
				// 동일한 키를 사용하는 항목이 이미 추가되었습니다.
				//    위치: System.ThrowHelper.ThrowArgumentException(ExceptionResource resource)
				//    위치: System.Collections.Generic.Dictionary`2.Insert(TKey key, TValue value, Boolean add)
				//    위치: System.Collections.Generic.Dictionary`2.System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<TKey,TValue>>.Add(KeyValuePair`2 keyValuePair)
				//    위치: ..AddRange[,](IDictionary`2 , IDictionary`2 ) 파일 c:\Builds\556\Behemoth\ReleaseBranch Production Build NT\Sources\OpenSource\Cecil.Decompiler\Common\Extensions.cs:줄 99
				//    위치: ..( ) 파일 c:\Builds\556\Behemoth\ReleaseBranch Production Build NT\Sources\OpenSource\Cecil.Decompiler\Decompiler\MethodSpecificContext.cs:줄 181
				//    위치: ..( ) 파일 c:\Builds\556\Behemoth\ReleaseBranch Production Build NT\Sources\OpenSource\Cecil.Decompiler\Steps\RebuildLambdaExpressions.cs:줄 66
				//    위치: ..( ) 파일 c:\Builds\556\Behemoth\ReleaseBranch Production Build NT\Sources\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:줄 89
				//    위치: ..Visit( ) 파일 c:\Builds\556\Behemoth\ReleaseBranch Production Build NT\Sources\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:줄 270
				//    위치: ..Visit[,]( ) 파일 c:\Builds\556\Behemoth\ReleaseBranch Production Build NT\Sources\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:줄 280
				//    위치: ..Visit( ) 파일 c:\Builds\556\Behemoth\ReleaseBranch Production Build NT\Sources\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:줄 316
				//    위치: ..( ) 파일 c:\Builds\556\Behemoth\ReleaseBranch Production Build NT\Sources\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:줄 604
				//    위치: ..( ) 파일 c:\Builds\556\Behemoth\ReleaseBranch Production Build NT\Sources\OpenSource\Cecil.Decompiler\Steps\RebuildLambdaExpressions.cs:줄 118
				//    위치: ..( ) 파일 c:\Builds\556\Behemoth\ReleaseBranch Production Build NT\Sources\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:줄 125
				//    위치: ..Visit( ) 파일 c:\Builds\556\Behemoth\ReleaseBranch Production Build NT\Sources\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:줄 270
				//    위치: ..( ) 파일 c:\Builds\556\Behemoth\ReleaseBranch Production Build NT\Sources\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:줄 523
				//    위치: ..( ) 파일 c:\Builds\556\Behemoth\ReleaseBranch Production Build NT\Sources\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:줄 97
				//    위치: ..Visit( ) 파일 c:\Builds\556\Behemoth\ReleaseBranch Production Build NT\Sources\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:줄 270
				//    위치: ..( ) 파일 c:\Builds\556\Behemoth\ReleaseBranch Production Build NT\Sources\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:줄 377
				//    위치: ..( ) 파일 c:\Builds\556\Behemoth\ReleaseBranch Production Build NT\Sources\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:줄 59
				//    위치: ..Visit( ) 파일 c:\Builds\556\Behemoth\ReleaseBranch Production Build NT\Sources\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:줄 270
				//    위치: ..Visit[,]( ) 파일 c:\Builds\556\Behemoth\ReleaseBranch Production Build NT\Sources\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:줄 280
				//    위치: ..Visit( ) 파일 c:\Builds\556\Behemoth\ReleaseBranch Production Build NT\Sources\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:줄 311
				//    위치: ..( ) 파일 c:\Builds\556\Behemoth\ReleaseBranch Production Build NT\Sources\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:줄 331
				//    위치: ..( ) 파일 c:\Builds\556\Behemoth\ReleaseBranch Production Build NT\Sources\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:줄 49
				//    위치: ..Visit( ) 파일 c:\Builds\556\Behemoth\ReleaseBranch Production Build NT\Sources\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:줄 270
				//    위치: ..( ) 파일 c:\Builds\556\Behemoth\ReleaseBranch Production Build NT\Sources\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:줄 355
				//    위치: ..( ) 파일 c:\Builds\556\Behemoth\ReleaseBranch Production Build NT\Sources\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:줄 55
				//    위치: ..Visit( ) 파일 c:\Builds\556\Behemoth\ReleaseBranch Production Build NT\Sources\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:줄 270
				//    위치: ..Visit[,]( ) 파일 c:\Builds\556\Behemoth\ReleaseBranch Production Build NT\Sources\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:줄 280
				//    위치: ..Visit( ) 파일 c:\Builds\556\Behemoth\ReleaseBranch Production Build NT\Sources\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:줄 311
				//    위치: ..( ) 파일 c:\Builds\556\Behemoth\ReleaseBranch Production Build NT\Sources\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:줄 331
				//    위치: ..( ,  ) 파일 c:\Builds\556\Behemoth\ReleaseBranch Production Build NT\Sources\OpenSource\Cecil.Decompiler\Steps\RebuildLambdaExpressions.cs:줄 130
				//    위치: ..(MethodBody ,  , ILanguage ) 파일 c:\Builds\556\Behemoth\ReleaseBranch Production Build NT\Sources\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:줄 88
				//    위치: ..(MethodBody , ILanguage ) 파일 c:\Builds\556\Behemoth\ReleaseBranch Production Build NT\Sources\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:줄 70
				//    위치: ..(MethodBody , & ) 파일 c:\Builds\556\Behemoth\ReleaseBranch Production Build NT\Sources\OpenSource\Cecil.Decompiler\Decompiler\PropertyDecompiler.cs:줄 345
				// 
				// mailto: JustDecompilePublicFeedback@telerik.com

			}
			set
			{
				this._certValidationCallback = value;
			}
		}

		public bool CheckCertificateRevocation
		{
			get
			{
				return this._checkCertRevocation;
			}
			set
			{
				this._checkCertRevocation = value;
			}
		}

		public SslProtocols EnabledSslProtocols
		{
			get
			{
				return this._enabledProtocols;
			}
			set
			{
				this._enabledProtocols = value;
			}
		}

		protected SslConfiguration(SslProtocols enabledSslProtocols, bool checkCertificateRevocation)
		{
			this._enabledProtocols = enabledSslProtocols;
			this._checkCertRevocation = checkCertificateRevocation;
		}
	}
}