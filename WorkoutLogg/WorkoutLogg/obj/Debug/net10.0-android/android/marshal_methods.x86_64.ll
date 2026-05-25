; ModuleID = 'marshal_methods.x86_64.ll'
source_filename = "marshal_methods.x86_64.ll"
target datalayout = "e-m:e-p270:32:32-p271:32:32-p272:64:64-i64:64-f80:128-n8:16:32:64-S128"
target triple = "x86_64-unknown-linux-android21"

%struct.MarshalMethodName = type {
	i64, ; uint64_t id
	ptr ; char* name
}

%struct.MarshalMethodsManagedClass = type {
	i32, ; uint32_t token
	ptr ; MonoClass klass
}

@assembly_image_cache = dso_local local_unnamed_addr global [402 x ptr] zeroinitializer, align 16

; Each entry maps hash of an assembly name to an index into the `assembly_image_cache` array
@assembly_image_cache_hashes = dso_local local_unnamed_addr constant [1206 x i64] [
	i64 u0x001e58127c546039, ; 0: lib_System.Globalization.dll.so => 42
	i64 u0x0024d0f62dee05bd, ; 1: Xamarin.KotlinX.Coroutines.Core.dll => 356
	i64 u0x0071cf2d27b7d61e, ; 2: lib_Xamarin.AndroidX.SwipeRefreshLayout.dll.so => 335
	i64 u0x00b3aadb3a4c4038, ; 3: lib_Refit.dll.so => 255
	i64 u0x01109b0e4d99e61f, ; 4: System.ComponentModel.Annotations.dll => 13
	i64 u0x014b43fdeb5d21ad, ; 5: Microsoft.AspNetCore.Authorization.Policy.dll => 193
	i64 u0x020f428300334897, ; 6: Grpc.Net.Client.dll => 186
	i64 u0x02123411c4e01926, ; 7: lib_Xamarin.AndroidX.Navigation.Runtime.dll.so => 323
	i64 u0x022e81ea9c46e03a, ; 8: lib_CommunityToolkit.Maui.Core.dll.so => 179
	i64 u0x022f31be406de945, ; 9: Microsoft.Extensions.Options.ConfigurationExtensions => 238
	i64 u0x02827b47e97f2378, ; 10: System.Security.Cryptography.Pkcs.dll => 266
	i64 u0x0284512fad379f7e, ; 11: System.Runtime.Handles => 107
	i64 u0x02a4c5a44384f885, ; 12: Microsoft.Extensions.Caching.Memory => 218
	i64 u0x02abedc11addc1ed, ; 13: lib_Mono.Android.Runtime.dll.so => 174
	i64 u0x02f55bf70672f5c8, ; 14: lib_System.IO.FileSystem.DriveInfo.dll.so => 48
	i64 u0x032267b2a94db371, ; 15: lib_Xamarin.AndroidX.AppCompat.dll.so => 277
	i64 u0x03621c804933a890, ; 16: System.Buffers => 7
	i64 u0x0363ac97a4cb84e6, ; 17: SQLitePCLRaw.provider.e_sqlite3.dll => 261
	i64 u0x0377283fc1d7573a, ; 18: Microsoft.AspNetCore.DataProtection.Abstractions.dll => 197
	i64 u0x0399610510a38a38, ; 19: lib_System.Private.DataContractSerialization.dll.so => 88
	i64 u0x043032f1d071fae0, ; 20: ru/Microsoft.Maui.Controls.resources => 384
	i64 u0x044440a55165631e, ; 21: lib-cs-Microsoft.Maui.Controls.resources.dll.so => 362
	i64 u0x046eb1581a80c6b0, ; 22: vi/Microsoft.Maui.Controls.resources => 390
	i64 u0x0470607fd33c32db, ; 23: Microsoft.IdentityModel.Abstractions.dll => 241
	i64 u0x047408741db2431a, ; 24: Xamarin.AndroidX.DynamicAnimation => 297
	i64 u0x0517ef04e06e9f76, ; 25: System.Net.Primitives => 72
	i64 u0x0565d18c6da3de38, ; 26: Xamarin.AndroidX.RecyclerView => 327
	i64 u0x057bf9fa9fb09f7c, ; 27: Microsoft.Data.Sqlite.dll => 212
	i64 u0x0581db89237110e9, ; 28: lib_System.Collections.dll.so => 12
	i64 u0x05989cb940b225a9, ; 29: Microsoft.Maui.dll => 247
	i64 u0x05a1c25e78e22d87, ; 30: lib_System.Runtime.CompilerServices.Unsafe.dll.so => 104
	i64 u0x05c27cf2b380bbf2, ; 31: lib_Microsoft.AspNetCore.Hosting.Server.Abstractions.dll.so => 199
	i64 u0x05ef98b6a1db882c, ; 32: lib_Microsoft.Data.Sqlite.dll.so => 212
	i64 u0x06076b5d2b581f08, ; 33: zh-HK/Microsoft.Maui.Controls.resources => 391
	i64 u0x06388ffe9f6c161a, ; 34: System.Xml.Linq.dll => 159
	i64 u0x06600c4c124cb358, ; 35: System.Configuration.dll => 19
	i64 u0x067f95c5ddab55b3, ; 36: lib_Xamarin.AndroidX.Fragment.Ktx.dll.so => 302
	i64 u0x0680a433c781bb3d, ; 37: Xamarin.AndroidX.Collection.Jvm => 284
	i64 u0x069fff96ec92a91d, ; 38: System.Xml.XPath.dll => 164
	i64 u0x070b0847e18dab68, ; 39: Xamarin.AndroidX.Emoji2.ViewsHelper.dll => 299
	i64 u0x072496def57d8011, ; 40: Microsoft.Extensions.WebEncoders.dll => 240
	i64 u0x0739448d84d3b016, ; 41: lib_Xamarin.AndroidX.VectorDrawable.dll.so => 339
	i64 u0x07469f2eecce9e85, ; 42: mscorlib.dll => 170
	i64 u0x07c57877c7ba78ad, ; 43: ru/Microsoft.Maui.Controls.resources.dll => 384
	i64 u0x07dcdc7460a0c5e4, ; 44: System.Collections.NonGeneric => 10
	i64 u0x08122e52765333c8, ; 45: lib_Microsoft.Extensions.Logging.Debug.dll.so => 235
	i64 u0x088610fc2509f69e, ; 46: lib_Xamarin.AndroidX.VectorDrawable.Animated.dll.so => 340
	i64 u0x08a7c865576bbde7, ; 47: System.Reflection.Primitives => 98
	i64 u0x08c9d051a4a817e5, ; 48: Xamarin.AndroidX.CustomView.PoolingContainer.dll => 295
	i64 u0x08f3c9788ee2153c, ; 49: Xamarin.AndroidX.DrawerLayout => 296
	i64 u0x09138715c92dba90, ; 50: lib_System.ComponentModel.Annotations.dll.so => 13
	i64 u0x0919c28b89381a0b, ; 51: lib_Microsoft.Extensions.Options.dll.so => 237
	i64 u0x092266563089ae3e, ; 52: lib_System.Collections.NonGeneric.dll.so => 10
	i64 u0x09d144a7e214d457, ; 53: System.Security.Cryptography => 129
	i64 u0x09d931c8a4087ae3, ; 54: lib_Microsoft.AspNetCore.DataProtection.Abstractions.dll.so => 197
	i64 u0x09e2b9f743db21a8, ; 55: lib_System.Reflection.Metadata.dll.so => 97
	i64 u0x0a805f95d98f597b, ; 56: lib_Microsoft.Extensions.Caching.Abstractions.dll.so => 217
	i64 u0x0a980941fa112bc4, ; 57: System.Security.Cryptography.Xml => 267
	i64 u0x0abb3e2b271edc45, ; 58: System.Threading.Channels.dll => 143
	i64 u0x0b06b1feab070143, ; 59: System.Formats.Tar => 39
	i64 u0x0b3b632c3bbee20c, ; 60: sk/Microsoft.Maui.Controls.resources => 385
	i64 u0x0b6aff547b84fbe9, ; 61: Xamarin.KotlinX.Serialization.Core.Jvm => 359
	i64 u0x0be1e582d0d8ef1a, ; 62: lib_Microsoft.AspNetCore.Cryptography.KeyDerivation.dll.so => 195
	i64 u0x0be2e1f8ce4064ed, ; 63: Xamarin.AndroidX.ViewPager => 342
	i64 u0x0c3ca6cc978e2aae, ; 64: pt-BR/Microsoft.Maui.Controls.resources => 381
	i64 u0x0c59ad9fbbd43abe, ; 65: Mono.Android => 175
	i64 u0x0c65741e86371ee3, ; 66: lib_Xamarin.Android.Glide.GifDecoder.dll.so => 271
	i64 u0x0c74af560004e816, ; 67: Microsoft.Win32.Registry.dll => 5
	i64 u0x0c7790f60165fc06, ; 68: lib_Microsoft.Maui.Essentials.dll.so => 248
	i64 u0x0c83c82812e96127, ; 69: lib_System.Net.Mail.dll.so => 68
	i64 u0x0cce4bce83380b7f, ; 70: Xamarin.AndroidX.Security.SecurityCrypto => 332
	i64 u0x0d13cd7cce4284e4, ; 71: System.Security.SecureString => 132
	i64 u0x0d34fb076d8103ae, ; 72: Microsoft.Extensions.Identity.Core.dll => 231
	i64 u0x0d565cb22b8879da, ; 73: lib_Grpc.Core.Api.dll.so => 185
	i64 u0x0d63f4f73521c24f, ; 74: lib_Xamarin.AndroidX.SavedState.SavedState.Ktx.dll.so => 331
	i64 u0x0e04e702012f8463, ; 75: Xamarin.AndroidX.Emoji2 => 298
	i64 u0x0e14e73a54dda68e, ; 76: lib_System.Net.NameResolution.dll.so => 69
	i64 u0x0f37dd7a62ae99af, ; 77: lib_Xamarin.AndroidX.Collection.Ktx.dll.so => 285
	i64 u0x0f5e7abaa7cf470a, ; 78: System.Net.HttpListener => 67
	i64 u0x0f948418e9ebd6de, ; 79: Microsoft.AspNetCore.Hosting.Abstractions.dll => 198
	i64 u0x1001f97bbe242e64, ; 80: System.IO.UnmanagedMemoryStream => 57
	i64 u0x102a31b45304b1da, ; 81: Xamarin.AndroidX.CustomView => 294
	i64 u0x1065c4cb554c3d75, ; 82: System.IO.IsolatedStorage.dll => 52
	i64 u0x10f6cfcbcf801616, ; 83: System.IO.Compression.Brotli => 43
	i64 u0x1140109eb2e77ceb, ; 84: Microsoft.Extensions.ObjectPool.dll => 236
	i64 u0x114443cdcf2091f1, ; 85: System.Security.Cryptography.Primitives => 127
	i64 u0x11a603952763e1d4, ; 86: System.Net.Mail => 68
	i64 u0x11a70d0e1009fb11, ; 87: System.Net.WebSockets.dll => 83
	i64 u0x11f26371eee0d3c1, ; 88: lib_Xamarin.AndroidX.Lifecycle.Runtime.Ktx.dll.so => 312
	i64 u0x12128b3f59302d47, ; 89: lib_System.Xml.Serialization.dll.so => 161
	i64 u0x123639456fb056da, ; 90: System.Reflection.Emit.Lightweight.dll => 94
	i64 u0x12521e9764603eaa, ; 91: lib_System.Resources.Reader.dll.so => 101
	i64 u0x125b7f94acb989db, ; 92: Xamarin.AndroidX.RecyclerView.dll => 327
	i64 u0x12d3b63863d4ab0b, ; 93: lib_System.Threading.Overlapped.dll.so => 144
	i64 u0x134eab1061c395ee, ; 94: System.Transactions => 154
	i64 u0x138567fa954faa55, ; 95: Xamarin.AndroidX.Browser => 281
	i64 u0x13a01de0cbc3f06c, ; 96: lib-fr-Microsoft.Maui.Controls.resources.dll.so => 368
	i64 u0x13beedefb0e28a45, ; 97: lib_System.Xml.XmlDocument.dll.so => 165
	i64 u0x13f1e5e209e91af4, ; 98: lib_Java.Interop.dll.so => 172
	i64 u0x13f1e880c25d96d1, ; 99: he/Microsoft.Maui.Controls.resources => 369
	i64 u0x143d8ea60a6a4011, ; 100: Microsoft.Extensions.DependencyInjection.Abstractions => 224
	i64 u0x1497051b917530bd, ; 101: lib_System.Net.WebSockets.dll.so => 83
	i64 u0x14d612a531c79c05, ; 102: Xamarin.JSpecify.dll => 353
	i64 u0x14e68447938213b7, ; 103: Xamarin.AndroidX.Collection.Ktx.dll => 285
	i64 u0x152a448bd1e745a7, ; 104: Microsoft.Win32.Primitives => 4
	i64 u0x1557de0138c445f4, ; 105: lib_Microsoft.Win32.Registry.dll.so => 5
	i64 u0x15bdc156ed462f2f, ; 106: lib_System.IO.FileSystem.dll.so => 51
	i64 u0x15e300c2c1668655, ; 107: System.Resources.Writer.dll => 103
	i64 u0x16054fdcb6b3098b, ; 108: Microsoft.Extensions.DependencyModel.dll => 225
	i64 u0x16bf2a22df043a09, ; 109: System.IO.Pipes.dll => 56
	i64 u0x16ea2b318ad2d830, ; 110: System.Security.Cryptography.Algorithms => 122
	i64 u0x16eeae54c7ebcc08, ; 111: System.Reflection.dll => 100
	i64 u0x17125c9a85b4929f, ; 112: lib_netstandard.dll.so => 171
	i64 u0x1716866f7416792e, ; 113: lib_System.Security.AccessControl.dll.so => 120
	i64 u0x174f71c46216e44a, ; 114: Xamarin.KotlinX.Coroutines.Core => 356
	i64 u0x1752c12f1e1fc00c, ; 115: System.Core => 21
	i64 u0x1791d47293d97a1b, ; 116: lib_Npgsql.EntityFrameworkCore.PostgreSQL.dll.so => 253
	i64 u0x17b56e25558a5d36, ; 117: lib-hu-Microsoft.Maui.Controls.resources.dll.so => 372
	i64 u0x17f9358913beb16a, ; 118: System.Text.Encodings.Web => 139
	i64 u0x17fc580bfd8cdf43, ; 119: lib_Modules.Common.Domain.dll.so => 395
	i64 u0x1809fb23f29ba44a, ; 120: lib_System.Reflection.TypeExtensions.dll.so => 99
	i64 u0x18402a709e357f3b, ; 121: lib_Xamarin.KotlinX.Serialization.Core.Jvm.dll.so => 359
	i64 u0x18a9befae51bb361, ; 122: System.Net.WebClient => 79
	i64 u0x18f0ce884e87d89a, ; 123: nb/Microsoft.Maui.Controls.resources.dll => 378
	i64 u0x18facb3695ca9224, ; 124: Refit.HttpClientFactory => 256
	i64 u0x191e65bd9d4de607, ; 125: Modules.Common.Grpc.Contacts => 396
	i64 u0x19777fba3c41b398, ; 126: Xamarin.AndroidX.Startup.StartupRuntime.dll => 334
	i64 u0x19a4c090f14ebb66, ; 127: System.Security.Claims => 121
	i64 u0x1a63352be1054efd, ; 128: Microsoft.AspNetCore.Hosting.Server.Abstractions.dll => 199
	i64 u0x1a91866a319e9259, ; 129: lib_System.Collections.Concurrent.dll.so => 8
	i64 u0x1aac34d1917ba5d3, ; 130: lib_System.dll.so => 168
	i64 u0x1aad60783ffa3e5b, ; 131: lib-th-Microsoft.Maui.Controls.resources.dll.so => 387
	i64 u0x1aea8f1c3b282172, ; 132: lib_System.Net.Ping.dll.so => 71
	i64 u0x1b4b7a1d0d265fa2, ; 133: Xamarin.Android.Glide.DiskLruCache => 270
	i64 u0x1bbdb16cfa73e785, ; 134: Xamarin.AndroidX.Lifecycle.Runtime.Ktx.Android => 313
	i64 u0x1bc766e07b2b4241, ; 135: Xamarin.AndroidX.ResourceInspection.Annotation.dll => 328
	i64 u0x1bea5a36aa1ed8de, ; 136: Microsoft.AspNetCore.Http.Extensions => 202
	i64 u0x1c292b1598348d77, ; 137: Microsoft.Extensions.Diagnostics.dll => 226
	i64 u0x1c753b5ff15bce1b, ; 138: Mono.Android.Runtime.dll => 174
	i64 u0x1cd47467799d8250, ; 139: System.Threading.Tasks.dll => 148
	i64 u0x1d23eafdc6dc346c, ; 140: System.Globalization.Calendars.dll => 40
	i64 u0x1da4110562816681, ; 141: Xamarin.AndroidX.Security.SecurityCrypto.dll => 332
	i64 u0x1db6820994506bf5, ; 142: System.IO.FileSystem.AccessControl.dll => 47
	i64 u0x1dba6509cc55b56f, ; 143: lib_Google.Protobuf.dll.so => 183
	i64 u0x1dbb0c2c6a999acb, ; 144: System.Diagnostics.StackTrace => 30
	i64 u0x1e3d87657e9659bc, ; 145: Xamarin.AndroidX.Navigation.UI => 325
	i64 u0x1e71143913d56c10, ; 146: lib-ko-Microsoft.Maui.Controls.resources.dll.so => 376
	i64 u0x1e7c31185e2fb266, ; 147: lib_System.Threading.Tasks.Parallel.dll.so => 147
	i64 u0x1ed8fcce5e9b50a0, ; 148: Microsoft.Extensions.Options.dll => 237
	i64 u0x1f055d15d807e1b2, ; 149: System.Xml.XmlSerializer => 166
	i64 u0x1f198ea93d5594b5, ; 150: Microsoft.Extensions.Identity.Core => 231
	i64 u0x1f1ed22c1085f044, ; 151: lib_System.Diagnostics.FileVersionInfo.dll.so => 28
	i64 u0x1f61df9c5b94d2c1, ; 152: lib_System.Numerics.dll.so => 86
	i64 u0x1f750bb5421397de, ; 153: lib_Xamarin.AndroidX.Tracing.Tracing.dll.so => 336
	i64 u0x1fd24a4951087d1f, ; 154: Microsoft.AspNetCore.Mvc.Core.dll => 207
	i64 u0x20237ea48006d7a8, ; 155: lib_System.Net.WebClient.dll.so => 79
	i64 u0x209375905fcc1bad, ; 156: lib_System.IO.Compression.Brotli.dll.so => 43
	i64 u0x20fab3cf2dfbc8df, ; 157: lib_System.Diagnostics.Process.dll.so => 29
	i64 u0x2110167c128cba15, ; 158: System.Globalization => 42
	i64 u0x21419508838f7547, ; 159: System.Runtime.CompilerServices.VisualC => 105
	i64 u0x2174319c0d835bc9, ; 160: System.Runtime => 119
	i64 u0x2198e5bc8b7153fa, ; 161: Xamarin.AndroidX.Annotation.Experimental.dll => 275
	i64 u0x219ea1b751a4dee4, ; 162: lib_System.IO.Compression.ZipFile.dll.so => 45
	i64 u0x21cc7e445dcd5469, ; 163: System.Reflection.Emit.ILGeneration => 93
	i64 u0x220fd4f2e7c48170, ; 164: th/Microsoft.Maui.Controls.resources => 387
	i64 u0x224538d85ed15a82, ; 165: System.IO.Pipes => 56
	i64 u0x22908438c6bed1af, ; 166: lib_System.Threading.Timer.dll.so => 151
	i64 u0x237be844f1f812c7, ; 167: System.Threading.Thread.dll => 149
	i64 u0x23807c59646ec4f3, ; 168: lib_Microsoft.EntityFrameworkCore.dll.so => 214
	i64 u0x23852b3bdc9f7096, ; 169: System.Resources.ResourceManager => 102
	i64 u0x23986dd7e5d4fc01, ; 170: System.IO.FileSystem.Primitives.dll => 49
	i64 u0x2407aef2bbe8fadf, ; 171: System.Console => 20
	i64 u0x240abe014b27e7d3, ; 172: Xamarin.AndroidX.Core.dll => 290
	i64 u0x247619fe4413f8bf, ; 173: System.Runtime.Serialization.Primitives.dll => 116
	i64 u0x24de8d301281575e, ; 174: Xamarin.Android.Glide => 268
	i64 u0x252073cc3caa62c2, ; 175: fr/Microsoft.Maui.Controls.resources.dll => 368
	i64 u0x256b8d41255f01b1, ; 176: Xamarin.Google.Crypto.Tink.Android => 349
	i64 u0x25a0a7eff76ea08e, ; 177: SQLitePCLRaw.batteries_v2.dll => 258
	i64 u0x2662c629b96b0b30, ; 178: lib_Xamarin.Kotlin.StdLib.dll.so => 354
	i64 u0x268c1439f13bcc29, ; 179: lib_Microsoft.Extensions.Primitives.dll.so => 239
	i64 u0x26a670e154a9c54b, ; 180: System.Reflection.Extensions.dll => 96
	i64 u0x26d077d9678fe34f, ; 181: System.IO.dll => 58
	i64 u0x270a44600c921861, ; 182: System.IdentityModel.Tokens.Jwt => 265
	i64 u0x273f3515de5faf0d, ; 183: id/Microsoft.Maui.Controls.resources.dll => 373
	i64 u0x2742545f9094896d, ; 184: hr/Microsoft.Maui.Controls.resources => 371
	i64 u0x274d85d83ad40513, ; 185: lib_Xamarin.AndroidX.Window.WindowCore.dll.so => 345
	i64 u0x2759af78ab94d39b, ; 186: System.Net.WebSockets => 83
	i64 u0x27b2b16f3e9de038, ; 187: Xamarin.Google.Crypto.Tink.Android.dll => 349
	i64 u0x27b410442fad6cf1, ; 188: Java.Interop.dll => 172
	i64 u0x27b97e0d52c3034a, ; 189: System.Diagnostics.Debug => 26
	i64 u0x2801845a2c71fbfb, ; 190: System.Net.Primitives.dll => 72
	i64 u0x286835e259162700, ; 191: lib_Xamarin.AndroidX.ProfileInstaller.ProfileInstaller.dll.so => 326
	i64 u0x288f0dc6b8b36b5f, ; 192: Refit.dll => 255
	i64 u0x28b311fffbc0f8df, ; 193: Microsoft.AspNetCore.WebUtilities => 211
	i64 u0x28e52865585a1ebe, ; 194: Microsoft.Extensions.Diagnostics.Abstractions.dll => 227
	i64 u0x2949f3617a02c6b2, ; 195: Xamarin.AndroidX.ExifInterface => 300
	i64 u0x29aeab763a527e52, ; 196: lib_Xamarin.AndroidX.Navigation.Common.Android.dll.so => 321
	i64 u0x2a128783efe70ba0, ; 197: uk/Microsoft.Maui.Controls.resources.dll => 389
	i64 u0x2a3b095612184159, ; 198: lib_System.Net.NetworkInformation.dll.so => 70
	i64 u0x2a6507a5ffabdf28, ; 199: System.Diagnostics.TraceSource.dll => 33
	i64 u0x2ad156c8e1354139, ; 200: fi/Microsoft.Maui.Controls.resources => 367
	i64 u0x2ad5d6b13b7a3e04, ; 201: System.ComponentModel.DataAnnotations.dll => 14
	i64 u0x2af298f63581d886, ; 202: System.Text.RegularExpressions.dll => 141
	i64 u0x2af615542f04da50, ; 203: System.IdentityModel.Tokens.Jwt.dll => 265
	i64 u0x2afc1c4f898552ee, ; 204: lib_System.Formats.Asn1.dll.so => 38
	i64 u0x2b148910ed40fbf9, ; 205: zh-Hant/Microsoft.Maui.Controls.resources.dll => 393
	i64 u0x2b6989d78cba9a15, ; 206: Xamarin.AndroidX.Concurrent.Futures.dll => 286
	i64 u0x2c40db0dbedda89b, ; 207: lib_Microsoft.AspNetCore.WebUtilities.dll.so => 211
	i64 u0x2c8bd14bb93a7d82, ; 208: lib-pl-Microsoft.Maui.Controls.resources.dll.so => 380
	i64 u0x2c9f5b50547a5125, ; 209: Modules.Users.Infrastructure.dll => 400
	i64 u0x2cbd9262ca785540, ; 210: lib_System.Text.Encoding.CodePages.dll.so => 136
	i64 u0x2cc9e1fed6257257, ; 211: lib_System.Reflection.Emit.Lightweight.dll.so => 94
	i64 u0x2cd723e9fe623c7c, ; 212: lib_System.Private.Xml.Linq.dll.so => 90
	i64 u0x2d169d318a968379, ; 213: System.Threading.dll => 152
	i64 u0x2d20145f27cfc1d2, ; 214: Xamarin.AndroidX.Window.WindowCore.Jvm.dll => 346
	i64 u0x2d47774b7d993f59, ; 215: sv/Microsoft.Maui.Controls.resources.dll => 386
	i64 u0x2d5ffcae1ad0aaca, ; 216: System.Data.dll => 24
	i64 u0x2db915caf23548d2, ; 217: System.Text.Json.dll => 140
	i64 u0x2dcaa0bb15a4117a, ; 218: System.IO.UnmanagedMemoryStream.dll => 57
	i64 u0x2e2ced2c3c6a1edc, ; 219: lib_System.Threading.AccessControl.dll.so => 142
	i64 u0x2e54220bd5db87a1, ; 220: Moduels.Workouts.DTO.dll => 394
	i64 u0x2e5a40c319acb800, ; 221: System.IO.FileSystem => 51
	i64 u0x2e6f1f226821322a, ; 222: el/Microsoft.Maui.Controls.resources.dll => 365
	i64 u0x2ed4e41fc62539c3, ; 223: StackExchange.Redis => 262
	i64 u0x2f02f94df3200fe5, ; 224: System.Diagnostics.Process => 29
	i64 u0x2f2e98e1c89b1aff, ; 225: System.Xml.ReaderWriter => 160
	i64 u0x2f5911d9ba814e4e, ; 226: System.Diagnostics.Tracing => 34
	i64 u0x2f84070a459bc31f, ; 227: lib_System.Xml.dll.so => 167
	i64 u0x2feb4d2fcda05cfd, ; 228: Microsoft.Extensions.Caching.Abstractions.dll => 217
	i64 u0x2ff49de6a71764a1, ; 229: lib_Microsoft.Extensions.Http.dll.so => 230
	i64 u0x3033937e1dfee52b, ; 230: FluentValidation => 182
	i64 u0x309ee9eeec09a71e, ; 231: lib_Xamarin.AndroidX.Fragment.dll.so => 301
	i64 u0x309f2bedefa9a318, ; 232: Microsoft.IdentityModel.Abstractions => 241
	i64 u0x30c6dda129408828, ; 233: System.IO.IsolatedStorage => 52
	i64 u0x31195fef5d8fb552, ; 234: _Microsoft.Android.Resource.Designer.dll => 401
	i64 u0x312c8ed623cbfc8d, ; 235: Xamarin.AndroidX.Window.dll => 344
	i64 u0x31496b779ed0663d, ; 236: lib_System.Reflection.DispatchProxy.dll.so => 92
	i64 u0x32243413e774362a, ; 237: Xamarin.AndroidX.CardView.dll => 282
	i64 u0x3235427f8d12dae1, ; 238: lib_System.Drawing.Primitives.dll.so => 35
	i64 u0x329753a17a517811, ; 239: fr/Microsoft.Maui.Controls.resources => 368
	i64 u0x32aa989ff07a84ff, ; 240: lib_System.Xml.ReaderWriter.dll.so => 160
	i64 u0x33829542f112d59b, ; 241: System.Collections.Immutable => 9
	i64 u0x33a31443733849fe, ; 242: lib-es-Microsoft.Maui.Controls.resources.dll.so => 366
	i64 u0x341abc357fbb4ebf, ; 243: lib_System.Net.Sockets.dll.so => 78
	i64 u0x346a212343615ac5, ; 244: lib_System.Linq.AsyncEnumerable.dll.so => 59
	i64 u0x3496c1e2dcaf5ecc, ; 245: lib_System.IO.Pipes.AccessControl.dll.so => 55
	i64 u0x34dfd74fe2afcf37, ; 246: Microsoft.Maui => 247
	i64 u0x34e292762d9615df, ; 247: cs/Microsoft.Maui.Controls.resources.dll => 362
	i64 u0x3508234247f48404, ; 248: Microsoft.Maui.Controls => 245
	i64 u0x353590da528c9d22, ; 249: System.ComponentModel.Annotations => 13
	i64 u0x3549870798b4cd30, ; 250: lib_Xamarin.AndroidX.ViewPager2.dll.so => 343
	i64 u0x355282fc1c909694, ; 251: Microsoft.Extensions.Configuration => 220
	i64 u0x3552fc5d578f0fbf, ; 252: Xamarin.AndroidX.Arch.Core.Common => 279
	i64 u0x355c649948d55d97, ; 253: lib_System.Runtime.Intrinsics.dll.so => 111
	i64 u0x3598b7b6237a86b6, ; 254: lib_Microsoft.AspNetCore.Authentication.dll.so => 188
	i64 u0x35ea9d1c6834bc8c, ; 255: Xamarin.AndroidX.Lifecycle.ViewModel.Ktx.dll => 316
	i64 u0x36263608556d5d42, ; 256: Npgsql.dll => 252
	i64 u0x3628ab68db23a01a, ; 257: lib_System.Diagnostics.Tools.dll.so => 32
	i64 u0x3673b042508f5b6b, ; 258: lib_System.Runtime.Extensions.dll.so => 106
	i64 u0x36740f1a8ecdc6c4, ; 259: System.Numerics => 86
	i64 u0x36b2b50fdf589ae2, ; 260: System.Reflection.Emit.Lightweight => 94
	i64 u0x36cada77dc79928b, ; 261: System.IO.MemoryMappedFiles => 53
	i64 u0x374ef46b06791af6, ; 262: System.Reflection.Primitives.dll => 98
	i64 u0x375a0c086b00470b, ; 263: Microsoft.AspNetCore.Authentication.dll => 188
	i64 u0x376bf93e521a5417, ; 264: lib_Xamarin.Jetbrains.Annotations.dll.so => 352
	i64 u0x37bc29f3183003b6, ; 265: lib_System.IO.dll.so => 58
	i64 u0x37fd73cba07e0b9d, ; 266: lib_Microsoft.AspNetCore.Cryptography.Internal.dll.so => 194
	i64 u0x380134e03b1e160a, ; 267: System.Collections.Immutable.dll => 9
	i64 u0x38049b5c59b39324, ; 268: System.Runtime.CompilerServices.Unsafe => 104
	i64 u0x385c17636bb6fe6e, ; 269: Xamarin.AndroidX.CustomView.dll => 294
	i64 u0x38869c811d74050e, ; 270: System.Net.NameResolution.dll => 69
	i64 u0x38f71e7a64343c93, ; 271: lib_Microsoft.AspNetCore.Authorization.Policy.dll.so => 193
	i64 u0x393c226616977fdb, ; 272: lib_Xamarin.AndroidX.ViewPager.dll.so => 342
	i64 u0x395e37c3334cf82a, ; 273: lib-ca-Microsoft.Maui.Controls.resources.dll.so => 361
	i64 u0x39aa39fda111d9d3, ; 274: Newtonsoft.Json => 251
	i64 u0x39c3107c28752af1, ; 275: lib_Microsoft.Extensions.FileProviders.Abstractions.dll.so => 228
	i64 u0x3ab5859054645f72, ; 276: System.Security.Cryptography.Primitives.dll => 127
	i64 u0x3ad75090c3fac0e9, ; 277: lib_Xamarin.AndroidX.ResourceInspection.Annotation.dll.so => 328
	i64 u0x3ae44ac43a1fbdbb, ; 278: System.Runtime.Serialization => 118
	i64 u0x3b860f9932505633, ; 279: lib_System.Text.Encoding.Extensions.dll.so => 137
	i64 u0x3be99b43dd39dd37, ; 280: Xamarin.AndroidX.SavedState.SavedState.Android => 330
	i64 u0x3bea9ebe8c027c01, ; 281: lib_Microsoft.IdentityModel.Tokens.dll.so => 244
	i64 u0x3c3aafb6b3a00bf6, ; 282: lib_System.Security.Cryptography.X509Certificates.dll.so => 128
	i64 u0x3c4049146b59aa90, ; 283: System.Runtime.InteropServices.JavaScript => 108
	i64 u0x3c7c495f58ac5ee9, ; 284: Xamarin.Kotlin.StdLib => 354
	i64 u0x3c7e5ed3d5db71bb, ; 285: System.Security => 133
	i64 u0x3ca05b43ec08224f, ; 286: Microsoft.AspNetCore.Http.Extensions.dll => 202
	i64 u0x3cd9d281d402eb9b, ; 287: Xamarin.AndroidX.Browser.dll => 281
	i64 u0x3d1c50cc001a991e, ; 288: Xamarin.Google.Guava.ListenableFuture.dll => 351
	i64 u0x3d2b1913edfc08d7, ; 289: lib_System.Threading.ThreadPool.dll.so => 150
	i64 u0x3d46f0b995082740, ; 290: System.Xml.Linq => 159
	i64 u0x3d8a8f400514a790, ; 291: Xamarin.AndroidX.Fragment.Ktx.dll => 302
	i64 u0x3d9c2a242b040a50, ; 292: lib_Xamarin.AndroidX.Core.dll.so => 290
	i64 u0x3da7781d6333a8fe, ; 293: SQLitePCLRaw.batteries_v2 => 258
	i64 u0x3daa14724d8f58e8, ; 294: Google.Protobuf.dll => 183
	i64 u0x3dbb6b9f5ab90fa7, ; 295: lib_Xamarin.AndroidX.DynamicAnimation.dll.so => 297
	i64 u0x3e5441657549b213, ; 296: Xamarin.AndroidX.ResourceInspection.Annotation => 328
	i64 u0x3e57d4d195c53c2e, ; 297: System.Reflection.TypeExtensions => 99
	i64 u0x3e580c35ecfc1247, ; 298: lib_Microsoft.AspNetCore.Http.dll.so => 200
	i64 u0x3e616ab4ed1f3f15, ; 299: lib_System.Data.dll.so => 24
	i64 u0x3f1d226e6e06db7e, ; 300: Xamarin.AndroidX.SlidingPaneLayout.dll => 333
	i64 u0x3f510adf788828dd, ; 301: System.Threading.Tasks.Extensions => 146
	i64 u0x3f6f5914291cdcf7, ; 302: Microsoft.Extensions.Hosting.Abstractions => 229
	i64 u0x407a10bb4bf95829, ; 303: lib_Xamarin.AndroidX.Navigation.Common.dll.so => 320
	i64 u0x40c98b6bd77346d4, ; 304: Microsoft.VisualBasic.dll => 3
	i64 u0x41640f0d7a3d1d80, ; 305: lib_Microsoft.Extensions.Caching.StackExchangeRedis.dll.so => 219
	i64 u0x41833cf766d27d96, ; 306: mscorlib => 170
	i64 u0x41cab042be111c34, ; 307: lib_Xamarin.AndroidX.AppCompat.AppCompatResources.dll.so => 278
	i64 u0x423a9ecc4d905a88, ; 308: lib_System.Resources.ResourceManager.dll.so => 102
	i64 u0x423bf51ae7def810, ; 309: System.Xml.XPath => 164
	i64 u0x42462ff15ddba223, ; 310: System.Resources.Reader.dll => 101
	i64 u0x4291015ff4e5ef71, ; 311: Xamarin.AndroidX.Core.ViewTree.dll => 292
	i64 u0x4294a05ba79b4e3b, ; 312: Microsoft.AspNetCore.Authentication.Cookies.dll => 190
	i64 u0x42a31b86e6ccc3f0, ; 313: System.Diagnostics.Contracts => 25
	i64 u0x42d76b1d438bed3f, ; 314: Microsoft.AspNetCore.Identity => 204
	i64 u0x430e95b891249788, ; 315: lib_System.Reflection.Emit.dll.so => 95
	i64 u0x43375950ec7c1b6a, ; 316: netstandard.dll => 171
	i64 u0x434c4e1d9284cdae, ; 317: Mono.Android.dll => 175
	i64 u0x43505013578652a0, ; 318: lib_Xamarin.AndroidX.Activity.Ktx.dll.so => 273
	i64 u0x437d06c381ed575a, ; 319: lib_Microsoft.VisualBasic.dll.so => 3
	i64 u0x43950f84de7cc79a, ; 320: pl/Microsoft.Maui.Controls.resources.dll => 380
	i64 u0x43c077442b230f64, ; 321: Xamarin.AndroidX.Tracing.Tracing.Android => 337
	i64 u0x43e8ca5bc927ff37, ; 322: lib_Xamarin.AndroidX.Emoji2.ViewsHelper.dll.so => 299
	i64 u0x448bd33429269b19, ; 323: Microsoft.CSharp => 1
	i64 u0x4499fa3c8e494654, ; 324: lib_System.Runtime.Serialization.Primitives.dll.so => 116
	i64 u0x4515080865a951a5, ; 325: Xamarin.Kotlin.StdLib.dll => 354
	i64 u0x45344658e8f1a46d, ; 326: Microsoft.AspNetCore.Authentication.Core => 191
	i64 u0x453c1277f85cf368, ; 327: lib_Microsoft.EntityFrameworkCore.Abstractions.dll.so => 215
	i64 u0x4545802489b736b9, ; 328: Xamarin.AndroidX.Fragment.Ktx => 302
	i64 u0x454b4d1e66bb783c, ; 329: Xamarin.AndroidX.Lifecycle.Process => 309
	i64 u0x458d2df79ac57c1d, ; 330: lib_System.IdentityModel.Tokens.Jwt.dll.so => 265
	i64 u0x45c40276a42e283e, ; 331: System.Diagnostics.TraceSource => 33
	i64 u0x45d443f2a29adc37, ; 332: System.AppContext.dll => 6
	i64 u0x45fcc9fd66f25095, ; 333: Microsoft.Extensions.DependencyModel => 225
	i64 u0x463d680a1dec0810, ; 334: System.Security.Cryptography.Xml.dll => 267
	i64 u0x46a4213bc97fe5ae, ; 335: lib-ru-Microsoft.Maui.Controls.resources.dll.so => 384
	i64 u0x47358bd471172e1d, ; 336: lib_System.Xml.Linq.dll.so => 159
	i64 u0x4747e19ad6a1d4bb, ; 337: Grpc.Net.Common => 187
	i64 u0x47daf4e1afbada10, ; 338: pt/Microsoft.Maui.Controls.resources => 382
	i64 u0x480c0a47dd42dd81, ; 339: lib_System.IO.MemoryMappedFiles.dll.so => 53
	i64 u0x497eb1d03ac05c8a, ; 340: lib_Microsoft.Extensions.WebEncoders.dll.so => 240
	i64 u0x49e952f19a4e2022, ; 341: System.ObjectModel => 87
	i64 u0x49ea01c721d701b5, ; 342: lib_Microsoft.Net.Http.Headers.dll.so => 250
	i64 u0x49f9e6948a8131e4, ; 343: lib_Xamarin.AndroidX.VersionedParcelable.dll.so => 341
	i64 u0x4a5667b2462a664b, ; 344: lib_Xamarin.AndroidX.Navigation.UI.dll.so => 325
	i64 u0x4a7a18981dbd56bc, ; 345: System.IO.Compression.FileSystem.dll => 44
	i64 u0x4aa5c60350917c06, ; 346: lib_Xamarin.AndroidX.Lifecycle.LiveData.Core.Ktx.dll.so => 308
	i64 u0x4b07a0ed0ab33ff4, ; 347: System.Runtime.Extensions.dll => 106
	i64 u0x4b576d47ac054f3c, ; 348: System.IO.FileSystem.AccessControl => 47
	i64 u0x4b5cc074fafbe58e, ; 349: Microsoft.AspNetCore.ResponseCaching.Abstractions => 208
	i64 u0x4b7b6532ded934b7, ; 350: System.Text.Json => 140
	i64 u0x4bbc7df476e2e556, ; 351: Microsoft.AspNetCore.Mvc.Abstractions.dll => 206
	i64 u0x4c2029a97af23a8d, ; 352: Xamarin.AndroidX.Lifecycle.ViewModelSavedState.Android => 318
	i64 u0x4c7755cf07ad2d5f, ; 353: System.Net.Http.Json.dll => 65
	i64 u0x4ca014ceac582c86, ; 354: Microsoft.EntityFrameworkCore.Relational.dll => 216
	i64 u0x4cc5f15266470798, ; 355: lib_Xamarin.AndroidX.Loader.dll.so => 319
	i64 u0x4cf6f67dc77aacd2, ; 356: System.Net.NetworkInformation.dll => 70
	i64 u0x4d3183dd245425d4, ; 357: System.Net.WebSockets.Client.dll => 82
	i64 u0x4d479f968a05e504, ; 358: System.Linq.Expressions.dll => 60
	i64 u0x4d55a010ffc4faff, ; 359: System.Private.Xml => 91
	i64 u0x4d5cbe77561c5b2e, ; 360: System.Web.dll => 157
	i64 u0x4d77512dbd86ee4c, ; 361: lib_Xamarin.AndroidX.Arch.Core.Common.dll.so => 279
	i64 u0x4d7793536e79c309, ; 362: System.ServiceProcess => 135
	i64 u0x4d95fccc1f67c7ca, ; 363: System.Runtime.Loader.dll => 112
	i64 u0x4db014bf0ff1c9c1, ; 364: System.Linq.AsyncEnumerable => 59
	i64 u0x4dcf44c3c9b076a2, ; 365: it/Microsoft.Maui.Controls.resources.dll => 374
	i64 u0x4dd9247f1d2c3235, ; 366: Xamarin.AndroidX.Loader.dll => 319
	i64 u0x4e2aeee78e2c4a87, ; 367: Xamarin.AndroidX.ProfileInstaller.ProfileInstaller => 326
	i64 u0x4e32f00cb0937401, ; 368: Mono.Android.Runtime => 174
	i64 u0x4e3369190c3dcd08, ; 369: Microsoft.Extensions.Identity.Stores => 232
	i64 u0x4e5eea4668ac2b18, ; 370: System.Text.Encoding.CodePages => 136
	i64 u0x4ebd0c4b82c5eefc, ; 371: lib_System.Threading.Channels.dll.so => 143
	i64 u0x4ee8eaa9c9c1151a, ; 372: System.Globalization.Calendars => 40
	i64 u0x4f21ee6ef9eb527e, ; 373: ca/Microsoft.Maui.Controls.resources => 361
	i64 u0x4fd5f3ee53d0a4f0, ; 374: SQLitePCLRaw.lib.e_sqlite3.android => 260
	i64 u0x4fdc964ec1888e25, ; 375: lib_Microsoft.Extensions.Configuration.Binder.dll.so => 222
	i64 u0x4ff55724131c908c, ; 376: lib_Microsoft.AspNetCore.Mvc.Core.dll.so => 207
	i64 u0x4ffd65baff757598, ; 377: Microsoft.IdentityModel.Tokens => 244
	i64 u0x5037f0be3c28c7a3, ; 378: lib_Microsoft.Maui.Controls.dll.so => 245
	i64 u0x508c1fa6b57728d9, ; 379: Grpc.Net.Common.dll => 187
	i64 u0x50c3a29b21050d45, ; 380: System.Linq.Parallel.dll => 61
	i64 u0x5112ed116d87baf8, ; 381: CommunityToolkit.Mvvm => 180
	i64 u0x5116b21580ae6eb0, ; 382: Microsoft.Extensions.Configuration.Binder.dll => 222
	i64 u0x5131bbe80989093f, ; 383: Xamarin.AndroidX.Lifecycle.ViewModel.Android.dll => 315
	i64 u0x516324a5050a7e3c, ; 384: System.Net.WebProxy => 81
	i64 u0x516d6f0b21a303de, ; 385: lib_System.Diagnostics.Contracts.dll.so => 25
	i64 u0x51bb8a2afe774e32, ; 386: System.Drawing => 36
	i64 u0x5216f09c5c4c95c8, ; 387: Microsoft.AspNetCore.Authentication.Abstractions => 189
	i64 u0x5247c5c32a4140f0, ; 388: System.Resources.Reader => 101
	i64 u0x526bb15e3c386364, ; 389: Xamarin.AndroidX.Lifecycle.Runtime.Ktx.dll => 312
	i64 u0x526ce79eb8e90527, ; 390: lib_System.Net.Primitives.dll.so => 72
	i64 u0x527497f521875686, ; 391: Microsoft.AspNetCore.Http.Abstractions => 201
	i64 u0x5277169428c6ebf6, ; 392: lib_Grpc.Net.Common.dll.so => 187
	i64 u0x52829f00b4467c38, ; 393: lib_System.Data.Common.dll.so => 22
	i64 u0x529ffe06f39ab8db, ; 394: Xamarin.AndroidX.Core => 290
	i64 u0x52ff996554dbf352, ; 395: Microsoft.Maui.Graphics => 249
	i64 u0x5324b9a9dedb24aa, ; 396: Microsoft.AspNetCore.Cryptography.Internal => 194
	i64 u0x535f7e40e8fef8af, ; 397: lib-sk-Microsoft.Maui.Controls.resources.dll.so => 385
	i64 u0x53978aac584c666e, ; 398: lib_System.Security.Cryptography.Cng.dll.so => 123
	i64 u0x53a96d5c86c9e194, ; 399: System.Net.NetworkInformation => 70
	i64 u0x53be1038a61e8d44, ; 400: System.Runtime.InteropServices.RuntimeInformation.dll => 109
	i64 u0x53c3014b9437e684, ; 401: lib-zh-HK-Microsoft.Maui.Controls.resources.dll.so => 391
	i64 u0x53d666fa678b6cea, ; 402: Microsoft.DotNet.PlatformAbstractions => 213
	i64 u0x5435e6f049e9bc37, ; 403: System.Security.Claims.dll => 121
	i64 u0x54795225dd1587af, ; 404: lib_System.Runtime.dll.so => 119
	i64 u0x547a34f14e5f6210, ; 405: Xamarin.AndroidX.Lifecycle.Common.dll => 304
	i64 u0x54a0124adceadbc7, ; 406: Microsoft.AspNetCore.DataProtection => 196
	i64 u0x54b851bc9b470503, ; 407: Xamarin.AndroidX.Navigation.Common.Android => 321
	i64 u0x556e8b63b660ab8b, ; 408: Xamarin.AndroidX.Lifecycle.Common.Jvm.dll => 305
	i64 u0x5588627c9a108ec9, ; 409: System.Collections.Specialized => 11
	i64 u0x55a898e4f42e3fae, ; 410: Microsoft.VisualBasic.Core.dll => 2
	i64 u0x55fa0c610fe93bb1, ; 411: lib_System.Security.Cryptography.OpenSsl.dll.so => 126
	i64 u0x56442b99bc64bb47, ; 412: System.Runtime.Serialization.Xml.dll => 117
	i64 u0x56a8b26e1aeae27b, ; 413: System.Threading.Tasks.Dataflow => 145
	i64 u0x56f932d61e93c07f, ; 414: System.Globalization.Extensions => 41
	i64 u0x571c5cfbec5ae8e2, ; 415: System.Private.Uri => 89
	i64 u0x576499c9f52fea31, ; 416: Xamarin.AndroidX.Annotation => 274
	i64 u0x578cd35c91d7b347, ; 417: lib_SQLitePCLRaw.core.dll.so => 259
	i64 u0x579a06fed6eec900, ; 418: System.Private.CoreLib.dll => 177
	i64 u0x57adda3c951abb33, ; 419: Microsoft.Extensions.Hosting.Abstractions.dll => 229
	i64 u0x57c542c14049b66d, ; 420: System.Diagnostics.DiagnosticSource => 27
	i64 u0x581a8bd5cfda563e, ; 421: System.Threading.Timer => 151
	i64 u0x584ac38e21d2fde1, ; 422: Microsoft.Extensions.Configuration.Binder => 222
	i64 u0x58601b2dda4a27b9, ; 423: lib-ja-Microsoft.Maui.Controls.resources.dll.so => 375
	i64 u0x58688d9af496b168, ; 424: Microsoft.Extensions.DependencyInjection.dll => 223
	i64 u0x587f59a16b329d9c, ; 425: Microsoft.Net.Http.Headers => 250
	i64 u0x588c167a79db6bfb, ; 426: lib_Xamarin.Google.ErrorProne.Annotations.dll.so => 350
	i64 u0x5906028ae5151104, ; 427: Xamarin.AndroidX.Activity.Ktx => 273
	i64 u0x595a356d23e8da9a, ; 428: lib_Microsoft.CSharp.dll.so => 1
	i64 u0x59c270386bf40142, ; 429: Microsoft.AspNetCore.Hosting.Server.Abstractions => 199
	i64 u0x59f9e60b9475085f, ; 430: lib_Xamarin.AndroidX.Annotation.Experimental.dll.so => 275
	i64 u0x5a745f5101a75527, ; 431: lib_System.IO.Compression.FileSystem.dll.so => 44
	i64 u0x5a89a886ae30258d, ; 432: lib_Xamarin.AndroidX.CoordinatorLayout.dll.so => 289
	i64 u0x5a8f6699f4a1caa9, ; 433: lib_System.Threading.dll.so => 152
	i64 u0x5ae9cd33b15841bf, ; 434: System.ComponentModel => 18
	i64 u0x5b41ce8de0a5118c, ; 435: WorkoutLogg => 0
	i64 u0x5b54391bdc6fcfe6, ; 436: System.Private.DataContractSerialization => 88
	i64 u0x5b5f0e240a06a2a2, ; 437: da/Microsoft.Maui.Controls.resources.dll => 363
	i64 u0x5b8109e8e14c5e3e, ; 438: System.Globalization.Extensions.dll => 41
	i64 u0x5bddd04d72a9e350, ; 439: Xamarin.AndroidX.Lifecycle.LiveData.Core.Ktx => 308
	i64 u0x5bdf16b09da116ab, ; 440: Xamarin.AndroidX.Collection => 283
	i64 u0x5beca398fe6dce6b, ; 441: lib_Microsoft.AspNetCore.Mvc.Abstractions.dll.so => 206
	i64 u0x5c019d5266093159, ; 442: lib_Xamarin.AndroidX.Lifecycle.Runtime.Ktx.Android.dll.so => 313
	i64 u0x5c30a4a35f9cc8c4, ; 443: lib_System.Reflection.Extensions.dll.so => 96
	i64 u0x5c393624b8176517, ; 444: lib_Microsoft.Extensions.Logging.dll.so => 233
	i64 u0x5c53c29f5073b0c9, ; 445: System.Diagnostics.FileVersionInfo => 28
	i64 u0x5c5633a83721db9f, ; 446: WorkoutLogg.dll => 0
	i64 u0x5c87463c575c7616, ; 447: lib_System.Globalization.Extensions.dll.so => 41
	i64 u0x5cbe0283eb598ee8, ; 448: Microsoft.AspNetCore.Routing => 209
	i64 u0x5d0a4a29b02d9d3c, ; 449: System.Net.WebHeaderCollection.dll => 80
	i64 u0x5d40c9b15181641f, ; 450: lib_Xamarin.AndroidX.Emoji2.dll.so => 298
	i64 u0x5d6ca10d35e9485b, ; 451: lib_Xamarin.AndroidX.Concurrent.Futures.dll.so => 286
	i64 u0x5d7ec76c1c703055, ; 452: System.Threading.Tasks.Parallel => 147
	i64 u0x5db0cbbd1028510e, ; 453: lib_System.Runtime.InteropServices.dll.so => 110
	i64 u0x5db30905d3e5013b, ; 454: Xamarin.AndroidX.Collection.Jvm.dll => 284
	i64 u0x5e467bc8f09ad026, ; 455: System.Collections.Specialized.dll => 11
	i64 u0x5e5173b3208d97e7, ; 456: System.Runtime.Handles.dll => 107
	i64 u0x5ea92fdb19ec8c4c, ; 457: System.Text.Encodings.Web.dll => 139
	i64 u0x5eb8046dd40e9ac3, ; 458: System.ComponentModel.Primitives => 16
	i64 u0x5ec272d219c9aba4, ; 459: System.Security.Cryptography.Csp.dll => 124
	i64 u0x5eee1376d94c7f5e, ; 460: System.Net.HttpListener.dll => 67
	i64 u0x5f36ccf5c6a57e24, ; 461: System.Xml.ReaderWriter.dll => 160
	i64 u0x5f4294b9b63cb842, ; 462: System.Data.Common => 22
	i64 u0x5f7399e166075632, ; 463: lib_SQLitePCLRaw.lib.e_sqlite3.android.dll.so => 260
	i64 u0x5f9a2d823f664957, ; 464: lib-el-Microsoft.Maui.Controls.resources.dll.so => 365
	i64 u0x5fa6da9c3cd8142a, ; 465: lib_Xamarin.KotlinX.Serialization.Core.dll.so => 358
	i64 u0x5fac98e0b37a5b9d, ; 466: System.Runtime.CompilerServices.Unsafe.dll => 104
	i64 u0x5fd02402d97cdaab, ; 467: lib_Microsoft.Extensions.ObjectPool.dll.so => 236
	i64 u0x609f4b7b63d802d4, ; 468: lib_Microsoft.Extensions.DependencyInjection.dll.so => 223
	i64 u0x60cd4e33d7e60134, ; 469: Xamarin.KotlinX.Coroutines.Core.Jvm => 357
	i64 u0x60f62d786afcf130, ; 470: System.Memory => 64
	i64 u0x61bb78c89f867353, ; 471: System.IO => 58
	i64 u0x61be8d1299194243, ; 472: Microsoft.Maui.Controls.Xaml => 246
	i64 u0x61d2cba29557038f, ; 473: de/Microsoft.Maui.Controls.resources => 364
	i64 u0x61d88f399afb2f45, ; 474: lib_System.Runtime.Loader.dll.so => 112
	i64 u0x622eef6f9e59068d, ; 475: System.Private.CoreLib => 177
	i64 u0x639fb99a7bef11de, ; 476: Xamarin.AndroidX.Navigation.Runtime.Android.dll => 324
	i64 u0x63d5e3aa4ef9b931, ; 477: Xamarin.KotlinX.Coroutines.Android.dll => 355
	i64 u0x63f1f6883c1e23c2, ; 478: lib_System.Collections.Immutable.dll.so => 9
	i64 u0x6400f68068c1e9f1, ; 479: Xamarin.Google.Android.Material.dll => 347
	i64 u0x640e3b14dbd325c2, ; 480: System.Security.Cryptography.Algorithms.dll => 122
	i64 u0x64587004560099b9, ; 481: System.Reflection => 100
	i64 u0x64b1529a438a3c45, ; 482: lib_System.Runtime.Handles.dll.so => 107
	i64 u0x64b61dd9da8a4d57, ; 483: System.Net.ServerSentEvents.dll => 76
	i64 u0x655a00a848fc3334, ; 484: lib_Modules.Users.Infrastructure.dll.so => 400
	i64 u0x6565fba2cd8f235b, ; 485: Xamarin.AndroidX.Lifecycle.ViewModel.Ktx => 316
	i64 u0x658f524e4aba7dad, ; 486: CommunityToolkit.Maui.dll => 178
	i64 u0x659dc45417570048, ; 487: Refit => 255
	i64 u0x65ecac39144dd3cc, ; 488: Microsoft.Maui.Controls.dll => 245
	i64 u0x65ece51227bfa724, ; 489: lib_System.Runtime.Numerics.dll.so => 113
	i64 u0x661722438787b57f, ; 490: Xamarin.AndroidX.Annotation.Jvm.dll => 276
	i64 u0x6679b2337ee6b22a, ; 491: lib_System.IO.FileSystem.Primitives.dll.so => 49
	i64 u0x667c66a03dd97d40, ; 492: System.Linq.AsyncEnumerable.dll => 59
	i64 u0x6692e924eade1b29, ; 493: lib_System.Console.dll.so => 20
	i64 u0x66a4e5c6a3fb0bae, ; 494: lib_Xamarin.AndroidX.Lifecycle.ViewModel.Android.dll.so => 315
	i64 u0x66d13304ce1a3efa, ; 495: Xamarin.AndroidX.CursorAdapter => 293
	i64 u0x674303f65d8fad6f, ; 496: lib_System.Net.Quic.dll.so => 73
	i64 u0x6756ca4cad62e9d6, ; 497: lib_Xamarin.AndroidX.ConstraintLayout.Core.dll.so => 288
	i64 u0x67c0802770244408, ; 498: System.Windows.dll => 158
	i64 u0x68100b69286e27cd, ; 499: lib_System.Formats.Tar.dll.so => 39
	i64 u0x68558ec653afa616, ; 500: lib-da-Microsoft.Maui.Controls.resources.dll.so => 363
	i64 u0x6872ec7a2e36b1ac, ; 501: System.Drawing.Primitives.dll => 35
	i64 u0x68fbbbe2eb455198, ; 502: System.Formats.Asn1 => 38
	i64 u0x69063fc0ba8e6bdd, ; 503: he/Microsoft.Maui.Controls.resources.dll => 369
	i64 u0x699dffb2427a2d71, ; 504: SQLitePCLRaw.lib.e_sqlite3.android.dll => 260
	i64 u0x6a4d7577b2317255, ; 505: System.Runtime.InteropServices.dll => 110
	i64 u0x6ace3b74b15ee4a4, ; 506: nb/Microsoft.Maui.Controls.resources => 378
	i64 u0x6afcedb171067e2b, ; 507: System.Core.dll => 21
	i64 u0x6ba55548e7efe195, ; 508: Confluent.Kafka => 181
	i64 u0x6bef98e124147c24, ; 509: Xamarin.Jetbrains.Annotations => 352
	i64 u0x6ce874bff138ce2b, ; 510: Xamarin.AndroidX.Lifecycle.ViewModel.dll => 314
	i64 u0x6d12bfaa99c72b1f, ; 511: lib_Microsoft.Maui.Graphics.dll.so => 249
	i64 u0x6d70755158ca866e, ; 512: lib_System.ComponentModel.EventBasedAsync.dll.so => 15
	i64 u0x6d79993361e10ef2, ; 513: Microsoft.Extensions.Primitives => 239
	i64 u0x6d7eeca99577fc8b, ; 514: lib_System.Net.WebProxy.dll.so => 81
	i64 u0x6d8515b19946b6a2, ; 515: System.Net.WebProxy.dll => 81
	i64 u0x6d86d56b84c8eb71, ; 516: lib_Xamarin.AndroidX.CursorAdapter.dll.so => 293
	i64 u0x6d9bea6b3e895cf7, ; 517: Microsoft.Extensions.Primitives.dll => 239
	i64 u0x6e098fb160d68d4f, ; 518: Modules.Users.Domain => 398
	i64 u0x6e25a02c3833319a, ; 519: lib_Xamarin.AndroidX.Navigation.Fragment.dll.so => 322
	i64 u0x6e79c6bd8627412a, ; 520: Xamarin.AndroidX.SavedState.SavedState.Ktx => 331
	i64 u0x6e838d9a2a6f6c9e, ; 521: lib_System.ValueTuple.dll.so => 155
	i64 u0x6e9965ce1095e60a, ; 522: lib_System.Core.dll.so => 21
	i64 u0x6fd2265da78b93a4, ; 523: lib_Microsoft.Maui.dll.so => 247
	i64 u0x6fdfc7de82c33008, ; 524: cs/Microsoft.Maui.Controls.resources => 362
	i64 u0x6ffc4967cc47ba57, ; 525: System.IO.FileSystem.Watcher.dll => 50
	i64 u0x701cd46a1c25a5fe, ; 526: System.IO.FileSystem.dll => 51
	i64 u0x70e99f48c05cb921, ; 527: tr/Microsoft.Maui.Controls.resources.dll => 388
	i64 u0x70fd3deda22442d2, ; 528: lib-nb-Microsoft.Maui.Controls.resources.dll.so => 378
	i64 u0x71485e7ffdb4b958, ; 529: System.Reflection.Extensions => 96
	i64 u0x7162a2fce67a945f, ; 530: lib_Xamarin.Android.Glide.Annotations.dll.so => 269
	i64 u0x717530326f808838, ; 531: lib_Microsoft.Extensions.Diagnostics.Abstractions.dll.so => 227
	i64 u0x71a495ea3761dde8, ; 532: lib-it-Microsoft.Maui.Controls.resources.dll.so => 374
	i64 u0x71ad672adbe48f35, ; 533: System.ComponentModel.Primitives.dll => 16
	i64 u0x71bc142d620e986a, ; 534: lib_System.Security.Cryptography.Pkcs.dll.so => 266
	i64 u0x720f102581a4a5c8, ; 535: Xamarin.AndroidX.Core.ViewTree => 292
	i64 u0x725f5a9e82a45c81, ; 536: System.Security.Cryptography.Encoding => 125
	i64 u0x72b1fb4109e08d7b, ; 537: lib-hr-Microsoft.Maui.Controls.resources.dll.so => 371
	i64 u0x72e0300099accce1, ; 538: System.Xml.XPath.XDocument => 163
	i64 u0x730bfb248998f67a, ; 539: System.IO.Compression.ZipFile => 45
	i64 u0x732b2d67b9e5c47b, ; 540: Xamarin.Google.ErrorProne.Annotations.dll => 350
	i64 u0x734b76fdc0dc05bb, ; 541: lib_GoogleGson.dll.so => 184
	i64 u0x73a6be34e822f9d1, ; 542: lib_System.Runtime.Serialization.dll.so => 118
	i64 u0x73e4ce94e2eb6ffc, ; 543: lib_System.Memory.dll.so => 64
	i64 u0x743a1eccf080489a, ; 544: WindowsBase.dll => 169
	i64 u0x7465c42afc9ef57e, ; 545: Microsoft.AspNetCore.Identity.EntityFrameworkCore => 205
	i64 u0x746cf89b511b4d40, ; 546: lib_Microsoft.Extensions.Diagnostics.dll.so => 226
	i64 u0x755a91767330b3d4, ; 547: lib_Microsoft.Extensions.Configuration.dll.so => 220
	i64 u0x75c326eb821b85c4, ; 548: lib_System.ComponentModel.DataAnnotations.dll.so => 14
	i64 u0x76012e7334db86e5, ; 549: lib_Xamarin.AndroidX.SavedState.dll.so => 329
	i64 u0x76ca07b878f44da0, ; 550: System.Runtime.Numerics.dll => 113
	i64 u0x7736c8a96e51a061, ; 551: lib_Xamarin.AndroidX.Annotation.Jvm.dll.so => 276
	i64 u0x778a805e625329ef, ; 552: System.Linq.Parallel => 61
	i64 u0x77d9074d8f33a303, ; 553: lib_System.Net.ServerSentEvents.dll.so => 76
	i64 u0x77f8a4acc2fdc449, ; 554: System.Security.Cryptography.Cng.dll => 123
	i64 u0x780bc73597a503a9, ; 555: lib-ms-Microsoft.Maui.Controls.resources.dll.so => 377
	i64 u0x782c5d8eb99ff201, ; 556: lib_Microsoft.VisualBasic.Core.dll.so => 2
	i64 u0x783606d1e53e7a1a, ; 557: th/Microsoft.Maui.Controls.resources.dll => 387
	i64 u0x78a45e51311409b6, ; 558: Xamarin.AndroidX.Fragment.dll => 301
	i64 u0x78ed4ab8f9d800a1, ; 559: Xamarin.AndroidX.Lifecycle.ViewModel => 314
	i64 u0x7a25bdb29108c6e7, ; 560: Microsoft.Extensions.Http => 230
	i64 u0x7a5207a7c82d30b4, ; 561: lib_Xamarin.JSpecify.dll.so => 353
	i64 u0x7a7e7eddf79c5d26, ; 562: lib_Xamarin.AndroidX.Lifecycle.ViewModel.dll.so => 314
	i64 u0x7a9a57d43b0845fa, ; 563: System.AppContext => 6
	i64 u0x7ad0f4f1e5d08183, ; 564: Xamarin.AndroidX.Collection.dll => 283
	i64 u0x7adb8da2ac89b647, ; 565: fi/Microsoft.Maui.Controls.resources.dll => 367
	i64 u0x7b13d9eaa944ade8, ; 566: Xamarin.AndroidX.DynamicAnimation.dll => 297
	i64 u0x7b150145c0a9058c, ; 567: Microsoft.Data.Sqlite => 212
	i64 u0x7b4927e421291c41, ; 568: Microsoft.IdentityModel.JsonWebTokens.dll => 242
	i64 u0x7bef86a4335c4870, ; 569: System.ComponentModel.TypeConverter => 17
	i64 u0x7c0820144cd34d6a, ; 570: sk/Microsoft.Maui.Controls.resources.dll => 385
	i64 u0x7c2a0bd1e0f988fc, ; 571: lib-de-Microsoft.Maui.Controls.resources.dll.so => 364
	i64 u0x7c41d387501568ba, ; 572: System.Net.WebClient.dll => 79
	i64 u0x7c482cd79bd24b13, ; 573: lib_Xamarin.AndroidX.ConstraintLayout.dll.so => 287
	i64 u0x7c60acf6404e96b6, ; 574: Xamarin.AndroidX.Navigation.Common.Android.dll => 321
	i64 u0x7c8f4b4f3731320f, ; 575: Pipelines.Sockets.Unofficial.dll => 254
	i64 u0x7cc637f941f716d0, ; 576: CommunityToolkit.Maui.Core => 179
	i64 u0x7cd2ec8eaf5241cd, ; 577: System.Security.dll => 133
	i64 u0x7cf9ae50dd350622, ; 578: Xamarin.Jetbrains.Annotations.dll => 352
	i64 u0x7d4040680e64c3ea, ; 579: Pipelines.Sockets.Unofficial => 254
	i64 u0x7d649b75d580bb42, ; 580: ms/Microsoft.Maui.Controls.resources.dll => 377
	i64 u0x7d832b3e1fe0edbb, ; 581: Modules.Common.Grpc.Contacts.dll => 396
	i64 u0x7d8ee2bdc8e3aad1, ; 582: System.Numerics.Vectors => 85
	i64 u0x7df5df8db8eaa6ac, ; 583: Microsoft.Extensions.Logging.Debug => 235
	i64 u0x7dfc3d6d9d8d7b70, ; 584: System.Collections => 12
	i64 u0x7e2e564fa2f76c65, ; 585: lib_System.Diagnostics.Tracing.dll.so => 34
	i64 u0x7e302e110e1e1346, ; 586: lib_System.Security.Claims.dll.so => 121
	i64 u0x7e4084a672f9c30e, ; 587: lib_System.Security.Cryptography.Xml.dll.so => 267
	i64 u0x7e4465b3f78ad8d0, ; 588: Xamarin.KotlinX.Serialization.Core.dll => 358
	i64 u0x7e571cad5915e6c3, ; 589: lib_Xamarin.AndroidX.Lifecycle.Process.dll.so => 309
	i64 u0x7e6ac99e4e8df72f, ; 590: System.IO.Hashing => 176
	i64 u0x7e6b1ca712437d7d, ; 591: Xamarin.AndroidX.Emoji2.ViewsHelper => 299
	i64 u0x7e946809d6008ef2, ; 592: lib_System.ObjectModel.dll.so => 87
	i64 u0x7ea0272c1b4a9635, ; 593: lib_Xamarin.Android.Glide.dll.so => 268
	i64 u0x7ebe6126501e1198, ; 594: Microsoft.AspNetCore.Cryptography.KeyDerivation.dll => 195
	i64 u0x7ecc13347c8fd849, ; 595: lib_System.ComponentModel.dll.so => 18
	i64 u0x7eff369f2e01cf95, ; 596: Microsoft.AspNetCore.Http.Features => 203
	i64 u0x7f00ddd9b9ca5a13, ; 597: Xamarin.AndroidX.ViewPager.dll => 342
	i64 u0x7f9351cd44b1273f, ; 598: Microsoft.Extensions.Configuration.Abstractions => 221
	i64 u0x7fbd557c99b3ce6f, ; 599: lib_Xamarin.AndroidX.Lifecycle.LiveData.Core.dll.so => 307
	i64 u0x7fd75077141d6658, ; 600: Microsoft.AspNetCore.Authorization.Policy => 193
	i64 u0x8076a9a44a2ca331, ; 601: System.Net.Quic => 73
	i64 u0x80da183a87731838, ; 602: System.Reflection.Metadata => 97
	i64 u0x80fa55b6d1b0be99, ; 603: SQLitePCLRaw.provider.e_sqlite3 => 261
	i64 u0x812c069d5cdecc17, ; 604: System.dll => 168
	i64 u0x81381be520a60adb, ; 605: Xamarin.AndroidX.Interpolator.dll => 303
	i64 u0x81657cec2b31e8aa, ; 606: System.Net => 84
	i64 u0x81ab745f6c0f5ce6, ; 607: zh-Hant/Microsoft.Maui.Controls.resources => 393
	i64 u0x822aa49008112ebe, ; 608: Microsoft.Extensions.ObjectPool => 236
	i64 u0x8235b6241d2f648b, ; 609: lib_Modules.Users.Domain.dll.so => 398
	i64 u0x8277f2be6b5ce05f, ; 610: Xamarin.AndroidX.AppCompat => 277
	i64 u0x828f06563b30bc50, ; 611: lib_Xamarin.AndroidX.CardView.dll.so => 282
	i64 u0x82b399cb01b531c4, ; 612: lib_System.Web.dll.so => 157
	i64 u0x82df8f5532a10c59, ; 613: lib_System.Drawing.dll.so => 36
	i64 u0x82f0b6e911d13535, ; 614: lib_System.Transactions.dll.so => 154
	i64 u0x82f6403342e12049, ; 615: uk/Microsoft.Maui.Controls.resources => 389
	i64 u0x83144699b312ad81, ; 616: SQLite-net.dll => 257
	i64 u0x83a7afd2c49adc86, ; 617: lib_Microsoft.IdentityModel.Abstractions.dll.so => 241
	i64 u0x83c14ba66c8e2b8c, ; 618: zh-Hans/Microsoft.Maui.Controls.resources => 392
	i64 u0x846ce984efea52c7, ; 619: System.Threading.Tasks.Parallel.dll => 147
	i64 u0x84ae73148a4557d2, ; 620: lib_System.IO.Pipes.dll.so => 56
	i64 u0x84b01102c12a9232, ; 621: System.Runtime.Serialization.Json.dll => 115
	i64 u0x84cd5cdec0f54bcc, ; 622: lib_Microsoft.EntityFrameworkCore.Relational.dll.so => 216
	i64 u0x84f20950c4c7164b, ; 623: Microsoft.AspNetCore.Http => 200
	i64 u0x850c5ba0b57ce8e7, ; 624: lib_Xamarin.AndroidX.Collection.dll.so => 283
	i64 u0x851d02edd334b044, ; 625: Xamarin.AndroidX.VectorDrawable => 339
	i64 u0x85c919db62150978, ; 626: Xamarin.AndroidX.Transition.dll => 338
	i64 u0x8662aaeb94fef37f, ; 627: lib_System.Dynamic.Runtime.dll.so => 37
	i64 u0x86a909228dc7657b, ; 628: lib-zh-Hant-Microsoft.Maui.Controls.resources.dll.so => 393
	i64 u0x86b3e00c36b84509, ; 629: Microsoft.Extensions.Configuration.dll => 220
	i64 u0x86b62cb077ec4fd7, ; 630: System.Runtime.Serialization.Xml => 117
	i64 u0x8706ffb12bf3f53d, ; 631: Xamarin.AndroidX.Annotation.Experimental => 275
	i64 u0x872a5b14c18d328c, ; 632: System.ComponentModel.DataAnnotations => 14
	i64 u0x872fb9615bc2dff0, ; 633: Xamarin.Android.Glide.Annotations.dll => 269
	i64 u0x87c4b8a492b176ad, ; 634: Microsoft.EntityFrameworkCore.Abstractions => 215
	i64 u0x87c69b87d9283884, ; 635: lib_System.Threading.Thread.dll.so => 149
	i64 u0x87d6cb5c641c5f07, ; 636: Microsoft.AspNetCore.Http.Abstractions.dll => 201
	i64 u0x87f6569b25707834, ; 637: System.IO.Compression.Brotli.dll => 43
	i64 u0x87fef727071b7fe5, ; 638: Grpc.Net.Client => 186
	i64 u0x8842b3a5d2d3fb36, ; 639: Microsoft.Maui.Essentials => 248
	i64 u0x88926583efe7ee86, ; 640: Xamarin.AndroidX.Activity.Ktx.dll => 273
	i64 u0x88ba6bc4f7762b03, ; 641: lib_System.Reflection.dll.so => 100
	i64 u0x88bda98e0cffb7a9, ; 642: lib_Xamarin.KotlinX.Coroutines.Core.Jvm.dll.so => 357
	i64 u0x88f629147ff1577f, ; 643: lib_Confluent.Kafka.dll.so => 181
	i64 u0x8930322c7bd8f768, ; 644: netstandard => 171
	i64 u0x897a606c9e39c75f, ; 645: lib_System.ComponentModel.Primitives.dll.so => 16
	i64 u0x898a5c6bc9e47ec1, ; 646: lib_Xamarin.AndroidX.SavedState.SavedState.Android.dll.so => 330
	i64 u0x898a9b4e63f2c138, ; 647: lib_Microsoft.AspNetCore.Identity.dll.so => 204
	i64 u0x89911a22005b92b7, ; 648: System.IO.FileSystem.DriveInfo.dll => 48
	i64 u0x89c5188089ec2cd5, ; 649: lib_System.Runtime.InteropServices.RuntimeInformation.dll.so => 109
	i64 u0x8a0b6f586fccda8a, ; 650: lib_Microsoft.AspNetCore.Http.Extensions.dll.so => 202
	i64 u0x8a14bf4400a024af, ; 651: lib_Microsoft.AspNetCore.Http.Features.dll.so => 203
	i64 u0x8a19e3dc71b34b2c, ; 652: System.Reflection.TypeExtensions.dll => 99
	i64 u0x8a399a706fcbce4b, ; 653: Microsoft.Extensions.Caching.Abstractions => 217
	i64 u0x8ad229ea26432ee2, ; 654: Xamarin.AndroidX.Loader => 319
	i64 u0x8b1b7008acd6fcc7, ; 655: Modules.Users.Infrastructure => 400
	i64 u0x8b4ff5d0fdd5faa1, ; 656: lib_System.Diagnostics.DiagnosticSource.dll.so => 27
	i64 u0x8b541d476eb3774c, ; 657: System.Security.Principal.Windows => 130
	i64 u0x8b8d01333a96d0b5, ; 658: System.Diagnostics.Process.dll => 29
	i64 u0x8b9ceca7acae3451, ; 659: lib-he-Microsoft.Maui.Controls.resources.dll.so => 369
	i64 u0x8bb8206f414c7c3b, ; 660: Microsoft.AspNetCore.Authentication.Core.dll => 191
	i64 u0x8c575135aa1ccef4, ; 661: Microsoft.Extensions.FileProviders.Abstractions => 228
	i64 u0x8cb8f612b633affb, ; 662: Xamarin.AndroidX.SavedState.SavedState.Ktx.dll => 331
	i64 u0x8cdfdb4ce85fb925, ; 663: lib_System.Security.Principal.Windows.dll.so => 130
	i64 u0x8cdfe7b8f4caa426, ; 664: System.IO.Compression.FileSystem => 44
	i64 u0x8d0f420977c2c1c7, ; 665: Xamarin.AndroidX.CursorAdapter.dll => 293
	i64 u0x8d52f7ea2796c531, ; 666: Xamarin.AndroidX.Emoji2.dll => 298
	i64 u0x8d7b8ab4b3310ead, ; 667: System.Threading => 152
	i64 u0x8da188285aadfe8e, ; 668: System.Collections.Concurrent => 8
	i64 u0x8dce248c34c54ef3, ; 669: lib_Microsoft.AspNetCore.Hosting.Abstractions.dll.so => 198
	i64 u0x8dfc1cfbf8858f95, ; 670: Grpc.Core.Api.dll => 185
	i64 u0x8e623fec9635e28f, ; 671: Syncfusion.Maui.Toolkit.resources.dll => 264
	i64 u0x8e8f269ad1e1ff94, ; 672: lib_Xamarin.AndroidX.Tracing.Tracing.Android.dll.so => 337
	i64 u0x8ec6e06a61c1baeb, ; 673: lib_Newtonsoft.Json.dll.so => 251
	i64 u0x8ed807bfe9858dfc, ; 674: Xamarin.AndroidX.Navigation.Common => 320
	i64 u0x8ee08b8194a30f48, ; 675: lib-hi-Microsoft.Maui.Controls.resources.dll.so => 370
	i64 u0x8ef7601039857a44, ; 676: lib-ro-Microsoft.Maui.Controls.resources.dll.so => 383
	i64 u0x8ef9414937d93a0a, ; 677: SQLitePCLRaw.core.dll => 259
	i64 u0x8f32c6f611f6ffab, ; 678: pt/Microsoft.Maui.Controls.resources.dll => 382
	i64 u0x8f44b45eb046bbd1, ; 679: System.ServiceModel.Web.dll => 134
	i64 u0x8f8829d21c8985a4, ; 680: lib-pt-BR-Microsoft.Maui.Controls.resources.dll.so => 381
	i64 u0x8f97020698a101ba, ; 681: Microsoft.AspNetCore.Routing.dll => 209
	i64 u0x8fbf5b0114c6dcef, ; 682: System.Globalization.dll => 42
	i64 u0x8fcc8c2a81f3d9e7, ; 683: Xamarin.KotlinX.Serialization.Core => 358
	i64 u0x8fd27d934d7b3a55, ; 684: SQLitePCLRaw.core => 259
	i64 u0x90263f8448b8f572, ; 685: lib_System.Diagnostics.TraceSource.dll.so => 33
	i64 u0x90281820febeff00, ; 686: lib_Microsoft.AspNetCore.Routing.Abstractions.dll.so => 210
	i64 u0x903101b46fb73a04, ; 687: _Microsoft.Android.Resource.Designer => 401
	i64 u0x90393bd4865292f3, ; 688: lib_System.IO.Compression.dll.so => 46
	i64 u0x905e2b8e7ae91ae6, ; 689: System.Threading.Tasks.Extensions.dll => 146
	i64 u0x90634f86c5ebe2b5, ; 690: Xamarin.AndroidX.Lifecycle.ViewModel.Android => 315
	i64 u0x907b636704ad79ef, ; 691: lib_Microsoft.Maui.Controls.Xaml.dll.so => 246
	i64 u0x90e9efbfd68593e0, ; 692: lib_Xamarin.AndroidX.Lifecycle.LiveData.dll.so => 306
	i64 u0x91418dc638b29e68, ; 693: lib_Xamarin.AndroidX.CustomView.dll.so => 294
	i64 u0x9157bd523cd7ed36, ; 694: lib_System.Text.Json.dll.so => 140
	i64 u0x91a74f07b30d37e2, ; 695: System.Linq.dll => 63
	i64 u0x91cb86ea3b17111d, ; 696: System.ServiceModel.Web => 134
	i64 u0x91fa41a87223399f, ; 697: ca/Microsoft.Maui.Controls.resources.dll => 361
	i64 u0x92054e486c0c7ea7, ; 698: System.IO.FileSystem.DriveInfo => 48
	i64 u0x928614058c40c4cd, ; 699: lib_System.Xml.XPath.XDocument.dll.so => 163
	i64 u0x92b138fffca2b01e, ; 700: lib_Xamarin.AndroidX.Arch.Core.Runtime.dll.so => 280
	i64 u0x92dd6c6033393bf7, ; 701: Syncfusion.Maui.Toolkit.resources => 264
	i64 u0x92dfc2bfc6c6a888, ; 702: Xamarin.AndroidX.Lifecycle.LiveData => 306
	i64 u0x933da2c779423d68, ; 703: Xamarin.Android.Glide.Annotations => 269
	i64 u0x9388aad9b7ae40ce, ; 704: lib_Xamarin.AndroidX.Lifecycle.Common.dll.so => 304
	i64 u0x93cfa73ab28d6e35, ; 705: ms/Microsoft.Maui.Controls.resources => 377
	i64 u0x941c00d21e5c0679, ; 706: lib_Xamarin.AndroidX.Transition.dll.so => 338
	i64 u0x942e2b32e944adec, ; 707: Modules.Users.Domain.dll => 398
	i64 u0x944077d8ca3c6580, ; 708: System.IO.Compression.dll => 46
	i64 u0x948cffedc8ed7960, ; 709: System.Xml => 167
	i64 u0x948d746a7702861f, ; 710: Microsoft.IdentityModel.Logging.dll => 243
	i64 u0x94bbeab0d4764588, ; 711: System.IO.Hashing.dll => 176
	i64 u0x94c8990839c4bdb1, ; 712: lib_Xamarin.AndroidX.Interpolator.dll.so => 303
	i64 u0x9564283c37ed59a9, ; 713: lib_Microsoft.IdentityModel.Logging.dll.so => 243
	i64 u0x957a4cdfdcfd6d83, ; 714: Refit.HttpClientFactory.dll => 256
	i64 u0x967fc325e09bfa8c, ; 715: es/Microsoft.Maui.Controls.resources => 366
	i64 u0x9686161486d34b81, ; 716: lib_Xamarin.AndroidX.ExifInterface.dll.so => 300
	i64 u0x9732d8dbddea3d9a, ; 717: id/Microsoft.Maui.Controls.resources => 373
	i64 u0x978be80e5210d31b, ; 718: Microsoft.Maui.Graphics.dll => 249
	i64 u0x97b8c771ea3e4220, ; 719: System.ComponentModel.dll => 18
	i64 u0x97e144c9d3c6976e, ; 720: System.Collections.Concurrent.dll => 8
	i64 u0x984184e3c70d4419, ; 721: GoogleGson => 184
	i64 u0x9843944103683dd3, ; 722: Xamarin.AndroidX.Core.Core.Ktx => 291
	i64 u0x9855609d6d191a4b, ; 723: lib_Modules.Common.Grpc.Contacts.dll.so => 396
	i64 u0x98b05cc81e6f333c, ; 724: Xamarin.AndroidX.SavedState.SavedState.Android.dll => 330
	i64 u0x98d720cc4597562c, ; 725: System.Security.Cryptography.OpenSsl => 126
	i64 u0x991d510397f92d9d, ; 726: System.Linq.Expressions => 60
	i64 u0x996ceeb8a3da3d67, ; 727: System.Threading.Overlapped.dll => 144
	i64 u0x999cb19e1a04ffd3, ; 728: CommunityToolkit.Mvvm.dll => 180
	i64 u0x99a00ca5270c6878, ; 729: Xamarin.AndroidX.Navigation.Runtime => 323
	i64 u0x99cdc6d1f2d3a72f, ; 730: ko/Microsoft.Maui.Controls.resources.dll => 376
	i64 u0x9a01b1da98b6ee10, ; 731: Xamarin.AndroidX.Lifecycle.Runtime.dll => 310
	i64 u0x9a5ccc274fd6e6ee, ; 732: Jsr305Binding.dll => 348
	i64 u0x9acfd25e735d5594, ; 733: lib_Npgsql.dll.so => 252
	i64 u0x9ae6940b11c02876, ; 734: lib_Xamarin.AndroidX.Window.dll.so => 344
	i64 u0x9b211a749105beac, ; 735: System.Transactions.Local => 153
	i64 u0x9b8734714671022d, ; 736: System.Threading.Tasks.Dataflow.dll => 145
	i64 u0x9bc6aea27fbf034f, ; 737: lib_Xamarin.KotlinX.Coroutines.Core.dll.so => 356
	i64 u0x9c244ac7cda32d26, ; 738: System.Security.Cryptography.X509Certificates.dll => 128
	i64 u0x9c465f280cf43733, ; 739: lib_Xamarin.KotlinX.Coroutines.Android.dll.so => 355
	i64 u0x9c6a130862518b21, ; 740: Modules.Common.Domain => 395
	i64 u0x9c8f6872beab6408, ; 741: System.Xml.XPath.XDocument.dll => 163
	i64 u0x9ce01cf91101ae23, ; 742: System.Xml.XmlDocument => 165
	i64 u0x9d128180c81d7ce6, ; 743: Xamarin.AndroidX.CustomView.PoolingContainer => 295
	i64 u0x9d5dbcf5a48583fe, ; 744: lib_Xamarin.AndroidX.Activity.dll.so => 272
	i64 u0x9d74dee1a7725f34, ; 745: Microsoft.Extensions.Configuration.Abstractions.dll => 221
	i64 u0x9dd0e195825d65c6, ; 746: lib_Xamarin.AndroidX.Navigation.Runtime.Android.dll.so => 324
	i64 u0x9e4534b6adaf6e84, ; 747: nl/Microsoft.Maui.Controls.resources => 379
	i64 u0x9e4b95dec42769f7, ; 748: System.Diagnostics.Debug.dll => 26
	i64 u0x9eaf1efdf6f7267e, ; 749: Xamarin.AndroidX.Navigation.Common.dll => 320
	i64 u0x9ef542cf1f78c506, ; 750: Xamarin.AndroidX.Lifecycle.LiveData.Core => 307
	i64 u0x9ffc74b9e35af6c6, ; 751: Modules.Users.DTO.dll => 399
	i64 u0xa00832eb975f56a8, ; 752: lib_System.Net.dll.so => 84
	i64 u0xa06617c0e4916b8f, ; 753: Microsoft.Extensions.Caching.StackExchangeRedis.dll => 219
	i64 u0xa0ad78236b7b267f, ; 754: Xamarin.AndroidX.Window => 344
	i64 u0xa0d8259f4cc284ec, ; 755: lib_System.Security.Cryptography.dll.so => 129
	i64 u0xa0e17ca50c77a225, ; 756: lib_Xamarin.Google.Crypto.Tink.Android.dll.so => 349
	i64 u0xa0ff9b3e34d92f11, ; 757: lib_System.Resources.Writer.dll.so => 103
	i64 u0xa12fbfb4da97d9f3, ; 758: System.Threading.Timer.dll => 151
	i64 u0xa13f33d9a41bee22, ; 759: FluentValidation.dll => 182
	i64 u0xa1440773ee9d341e, ; 760: Xamarin.Google.Android.Material => 347
	i64 u0xa18c39c44cdc3465, ; 761: Xamarin.AndroidX.Window.WindowCore => 345
	i64 u0xa1b9d7c27f47219f, ; 762: Xamarin.AndroidX.Navigation.UI.dll => 325
	i64 u0xa1cfec8d4a8d7c32, ; 763: Npgsql.EntityFrameworkCore.PostgreSQL.dll => 253
	i64 u0xa2572680829d2c7c, ; 764: System.IO.Pipelines.dll => 54
	i64 u0xa26597e57ee9c7f6, ; 765: System.Xml.XmlDocument.dll => 165
	i64 u0xa308401900e5bed3, ; 766: lib_mscorlib.dll.so => 170
	i64 u0xa395572e7da6c99d, ; 767: lib_System.Security.dll.so => 133
	i64 u0xa3c64c49e90a9987, ; 768: System.Security.Cryptography.Pkcs => 266
	i64 u0xa3e683f24b43af6f, ; 769: System.Dynamic.Runtime.dll => 37
	i64 u0xa4145becdee3dc4f, ; 770: Xamarin.AndroidX.VectorDrawable.Animated => 340
	i64 u0xa46aa1eaa214539b, ; 771: ko/Microsoft.Maui.Controls.resources => 376
	i64 u0xa473938c792db0b6, ; 772: Microsoft.AspNetCore.Routing.Abstractions => 210
	i64 u0xa4a372eecb9e4df0, ; 773: Microsoft.Extensions.Diagnostics => 226
	i64 u0xa4d20d2ff0563d26, ; 774: lib_CommunityToolkit.Mvvm.dll.so => 180
	i64 u0xa4edc8f2ceae241a, ; 775: System.Data.Common.dll => 22
	i64 u0xa5494f40f128ce6a, ; 776: System.Runtime.Serialization.Formatters.dll => 114
	i64 u0xa54b74df83dce92b, ; 777: System.Reflection.DispatchProxy => 92
	i64 u0xa5b7152421ed6d98, ; 778: lib_System.IO.FileSystem.Watcher.dll.so => 50
	i64 u0xa5c3844f17b822db, ; 779: lib_System.Linq.Parallel.dll.so => 61
	i64 u0xa5ce5c755bde8cb8, ; 780: lib_System.Security.Cryptography.Csp.dll.so => 124
	i64 u0xa5e599d1e0524750, ; 781: System.Numerics.Vectors.dll => 85
	i64 u0xa5f1ba49b85dd355, ; 782: System.Security.Cryptography.dll => 129
	i64 u0xa60fdaa9af524b6a, ; 783: Microsoft.DotNet.PlatformAbstractions.dll => 213
	i64 u0xa61975a5a37873ea, ; 784: lib_System.Xml.XmlSerializer.dll.so => 166
	i64 u0xa6593e21584384d2, ; 785: lib_Jsr305Binding.dll.so => 348
	i64 u0xa66cbee0130865f7, ; 786: lib_WindowsBase.dll.so => 169
	i64 u0xa67dbee13e1df9ca, ; 787: Xamarin.AndroidX.SavedState.dll => 329
	i64 u0xa684b098dd27b296, ; 788: lib_Xamarin.AndroidX.Security.SecurityCrypto.dll.so => 332
	i64 u0xa68a420042bb9b1f, ; 789: Xamarin.AndroidX.DrawerLayout.dll => 296
	i64 u0xa6d26156d1cacc7c, ; 790: Xamarin.Android.Glide.dll => 268
	i64 u0xa75386b5cb9595aa, ; 791: Xamarin.AndroidX.Lifecycle.Runtime.Android => 311
	i64 u0xa75cf331ee476318, ; 792: lib_Microsoft.AspNetCore.Http.Abstractions.dll.so => 201
	i64 u0xa763fbb98df8d9fb, ; 793: lib_Microsoft.Win32.Primitives.dll.so => 4
	i64 u0xa78ce3745383236a, ; 794: Xamarin.AndroidX.Lifecycle.Common.Jvm => 305
	i64 u0xa7c31b56b4dc7b33, ; 795: hu/Microsoft.Maui.Controls.resources => 372
	i64 u0xa7eab29ed44b4e7a, ; 796: Mono.Android.Export => 173
	i64 u0xa8195217cbf017b7, ; 797: Microsoft.VisualBasic.Core => 2
	i64 u0xa859a95830f367ff, ; 798: lib_Xamarin.AndroidX.Lifecycle.ViewModel.Ktx.dll.so => 316
	i64 u0xa8b52f21e0dbe690, ; 799: System.Runtime.Serialization.dll => 118
	i64 u0xa8bb61a1ac3eba47, ; 800: lib_Modules.Common.Infrastructure.dll.so => 397
	i64 u0xa8e6320dd07580ef, ; 801: lib_Microsoft.IdentityModel.JsonWebTokens.dll.so => 242
	i64 u0xa8ee4ed7de2efaee, ; 802: Xamarin.AndroidX.Annotation.dll => 274
	i64 u0xa95590e7c57438a4, ; 803: System.Configuration => 19
	i64 u0xa964304b5631e28a, ; 804: CommunityToolkit.Maui.Core.dll => 179
	i64 u0xaa2219c8e3449ff5, ; 805: Microsoft.Extensions.Logging.Abstractions => 234
	i64 u0xaa443ac34067eeef, ; 806: System.Private.Xml.dll => 91
	i64 u0xaa52de307ef5d1dd, ; 807: System.Net.Http => 66
	i64 u0xaa9a7b0214a5cc5c, ; 808: System.Diagnostics.StackTrace.dll => 30
	i64 u0xaaaf86367285a918, ; 809: Microsoft.Extensions.DependencyInjection.Abstractions.dll => 224
	i64 u0xaaf84bb3f052a265, ; 810: el/Microsoft.Maui.Controls.resources => 365
	i64 u0xab9af77b5b67a0b8, ; 811: Xamarin.AndroidX.ConstraintLayout.Core => 288
	i64 u0xab9c1b2687d86b0b, ; 812: lib_System.Linq.Expressions.dll.so => 60
	i64 u0xabaabcb70f9474ff, ; 813: Microsoft.AspNetCore.Mvc.Abstractions => 206
	i64 u0xac2af3fa195a15ce, ; 814: System.Runtime.Numerics => 113
	i64 u0xac5376a2a538dc10, ; 815: Xamarin.AndroidX.Lifecycle.LiveData.Core.dll => 307
	i64 u0xac5acae88f60357e, ; 816: System.Diagnostics.Tools.dll => 32
	i64 u0xac65e40f62b6b90e, ; 817: Google.Protobuf => 183
	i64 u0xac79c7e46047ad98, ; 818: System.Security.Principal.Windows.dll => 130
	i64 u0xac98d31068e24591, ; 819: System.Xml.XDocument => 162
	i64 u0xacd46e002c3ccb97, ; 820: ro/Microsoft.Maui.Controls.resources => 383
	i64 u0xacd4f3866b293bb7, ; 821: Microsoft.AspNetCore.Authentication.Cookies => 190
	i64 u0xacdd9e4180d56dda, ; 822: Xamarin.AndroidX.Concurrent.Futures => 286
	i64 u0xacf42eea7ef9cd12, ; 823: System.Threading.Channels => 143
	i64 u0xad89c07347f1bad6, ; 824: nl/Microsoft.Maui.Controls.resources.dll => 379
	i64 u0xadbb53caf78a79d2, ; 825: System.Web.HttpUtility => 156
	i64 u0xadc90ab061a9e6e4, ; 826: System.ComponentModel.TypeConverter.dll => 17
	i64 u0xadca1b9030b9317e, ; 827: Xamarin.AndroidX.Collection.Ktx => 285
	i64 u0xadd8eda2edf396ad, ; 828: Xamarin.Android.Glide.GifDecoder => 271
	i64 u0xadf4cf30debbeb9a, ; 829: System.Net.ServicePoint.dll => 77
	i64 u0xadf511667bef3595, ; 830: System.Net.Security => 75
	i64 u0xae0aaa94fdcfce0f, ; 831: System.ComponentModel.EventBasedAsync.dll => 15
	i64 u0xae282bcd03739de7, ; 832: Java.Interop => 172
	i64 u0xae53579c90db1107, ; 833: System.ObjectModel.dll => 87
	i64 u0xae7ea18c61eef394, ; 834: SQLite-net => 257
	i64 u0xaf4829c0b3e740ae, ; 835: lib_Syncfusion.Maui.Toolkit.resources.dll.so => 264
	i64 u0xaf732d0b2193b8f5, ; 836: System.Security.Cryptography.OpenSsl.dll => 126
	i64 u0xafdb94dbccd9d11c, ; 837: Xamarin.AndroidX.Lifecycle.LiveData.dll => 306
	i64 u0xafe29f45095518e7, ; 838: lib_Xamarin.AndroidX.Lifecycle.ViewModelSavedState.dll.so => 317
	i64 u0xb03ae931fb25607e, ; 839: Xamarin.AndroidX.ConstraintLayout => 287
	i64 u0xb05cc42cd94c6d9d, ; 840: lib-sv-Microsoft.Maui.Controls.resources.dll.so => 386
	i64 u0xb0ac21bec8f428c5, ; 841: Xamarin.AndroidX.Lifecycle.Runtime.Ktx.Android.dll => 313
	i64 u0xb0bb43dc52ea59f9, ; 842: System.Diagnostics.Tracing.dll => 34
	i64 u0xb110d64b6c9fbe46, ; 843: lib_Microsoft.Extensions.Identity.Core.dll.so => 231
	i64 u0xb1dd05401aa8ee63, ; 844: System.Security.AccessControl => 120
	i64 u0xb220631954820169, ; 845: System.Text.RegularExpressions => 141
	i64 u0xb2376e1dbf8b4ed7, ; 846: System.Security.Cryptography.Csp => 124
	i64 u0xb2a1959fe95c5402, ; 847: lib_System.Runtime.InteropServices.JavaScript.dll.so => 108
	i64 u0xb2a3f67f3bf29fce, ; 848: da/Microsoft.Maui.Controls.resources => 363
	i64 u0xb3874072ee0ecf8c, ; 849: Xamarin.AndroidX.VectorDrawable.Animated.dll => 340
	i64 u0xb3f0a0fcda8d3ebc, ; 850: Xamarin.AndroidX.CardView => 282
	i64 u0xb46be1aa6d4fff93, ; 851: hi/Microsoft.Maui.Controls.resources => 370
	i64 u0xb477491be13109d8, ; 852: ar/Microsoft.Maui.Controls.resources => 360
	i64 u0xb4ba8ad4cb954eb3, ; 853: Modules.Common.Infrastructure => 397
	i64 u0xb4bd7015ecee9d86, ; 854: System.IO.Pipelines => 54
	i64 u0xb4c53d9749c5f226, ; 855: lib_System.IO.FileSystem.AccessControl.dll.so => 47
	i64 u0xb4ff710863453fda, ; 856: System.Diagnostics.FileVersionInfo.dll => 28
	i64 u0xb50d9ae4eea71e97, ; 857: lib_Microsoft.DotNet.PlatformAbstractions.dll.so => 213
	i64 u0xb52aa297a3a175b1, ; 858: lib_Microsoft.AspNetCore.Authentication.Core.dll.so => 191
	i64 u0xb54092076b15e062, ; 859: System.Threading.AccessControl => 142
	i64 u0xb545f78b0415b9b9, ; 860: Microsoft.AspNetCore.WebUtilities.dll => 211
	i64 u0xb5c38bf497a4cfe2, ; 861: lib_System.Threading.Tasks.dll.so => 148
	i64 u0xb5c7fcdafbc67ee4, ; 862: Microsoft.Extensions.Logging.Abstractions.dll => 234
	i64 u0xb5dc0290c441c648, ; 863: lib_Microsoft.AspNetCore.Authentication.Cookies.dll.so => 190
	i64 u0xb5e59badb43e7829, ; 864: Microsoft.AspNetCore.Routing.Abstractions.dll => 210
	i64 u0xb5ea31d5244c6626, ; 865: System.Threading.ThreadPool.dll => 150
	i64 u0xb7212c4683a94afe, ; 866: System.Drawing.Primitives => 35
	i64 u0xb7b7753d1f319409, ; 867: sv/Microsoft.Maui.Controls.resources => 386
	i64 u0xb7fb824ee514af57, ; 868: Modules.Common.Domain.dll => 395
	i64 u0xb81a2c6e0aee50fe, ; 869: lib_System.Private.CoreLib.dll.so => 177
	i64 u0xb872c26142d22aa9, ; 870: Microsoft.Extensions.Http.dll => 230
	i64 u0xb8c60af47c08d4da, ; 871: System.Net.ServicePoint => 77
	i64 u0xb8e68d20aad91196, ; 872: lib_System.Xml.XPath.dll.so => 164
	i64 u0xb90ff82c284e9af9, ; 873: Grpc.Core.Api => 185
	i64 u0xb9185c33a1643eed, ; 874: Microsoft.CSharp.dll => 1
	i64 u0xb95c522c772254d2, ; 875: Microsoft.AspNetCore.DataProtection.dll => 196
	i64 u0xb960d6b2200ba320, ; 876: Xamarin.AndroidX.Lifecycle.ViewModelSavedState.Android.dll => 318
	i64 u0xb9b8001adf4ed7cc, ; 877: lib_Xamarin.AndroidX.SlidingPaneLayout.dll.so => 333
	i64 u0xb9f64d3b230def68, ; 878: lib-pt-Microsoft.Maui.Controls.resources.dll.so => 382
	i64 u0xb9fc3c8a556e3691, ; 879: ja/Microsoft.Maui.Controls.resources => 375
	i64 u0xba4670aa94a2b3c6, ; 880: lib_System.Xml.XDocument.dll.so => 162
	i64 u0xba48785529705af9, ; 881: System.Collections.dll => 12
	i64 u0xba965b8c86359996, ; 882: lib_System.Windows.dll.so => 158
	i64 u0xbb286883bc35db36, ; 883: System.Transactions.dll => 154
	i64 u0xbb639e0337b3d979, ; 884: Microsoft.AspNetCore.Http.dll => 200
	i64 u0xbb65706fde942ce3, ; 885: System.Net.Sockets => 78
	i64 u0xbba28979413cad9e, ; 886: lib_System.Runtime.CompilerServices.VisualC.dll.so => 105
	i64 u0xbbd180354b67271a, ; 887: System.Runtime.Serialization.Formatters => 114
	i64 u0xbbd599e40ecbe2a6, ; 888: Modules.Common.Infrastructure.dll => 397
	i64 u0xbc22a245dab70cb4, ; 889: lib_SQLitePCLRaw.provider.e_sqlite3.dll.so => 261
	i64 u0xbc260cdba33291a3, ; 890: Xamarin.AndroidX.Arch.Core.Common.dll => 279
	i64 u0xbcd36316d29f27b4, ; 891: lib_Microsoft.AspNetCore.Authorization.dll.so => 192
	i64 u0xbd0e2c0d55246576, ; 892: System.Net.Http.dll => 66
	i64 u0xbd3fbd85b9e1cb29, ; 893: lib_System.Net.HttpListener.dll.so => 67
	i64 u0xbd437a2cdb333d0d, ; 894: Xamarin.AndroidX.ViewPager2 => 343
	i64 u0xbd4f572d2bd0a789, ; 895: System.IO.Compression.ZipFile.dll => 45
	i64 u0xbd5d0b88d3d647a5, ; 896: lib_Xamarin.AndroidX.Browser.dll.so => 281
	i64 u0xbd770a375f100c23, ; 897: lib_Pipelines.Sockets.Unofficial.dll.so => 254
	i64 u0xbd877b14d0b56392, ; 898: System.Runtime.Intrinsics.dll => 111
	i64 u0xbde4cd9bb9008cb3, ; 899: lib_Microsoft.AspNetCore.Authentication.Abstractions.dll.so => 189
	i64 u0xbe4450ecf4d84c63, ; 900: lib_Microsoft.AspNetCore.ResponseCaching.Abstractions.dll.so => 208
	i64 u0xbe65a49036345cf4, ; 901: lib_System.Buffers.dll.so => 7
	i64 u0xbee38d4a88835966, ; 902: Xamarin.AndroidX.AppCompat.AppCompatResources => 278
	i64 u0xbef9919db45b4ca7, ; 903: System.IO.Pipes.AccessControl => 55
	i64 u0xbf0fa68611139208, ; 904: lib_Xamarin.AndroidX.Annotation.dll.so => 274
	i64 u0xbf677a56a0f14616, ; 905: Microsoft.AspNetCore.Authentication => 188
	i64 u0xbfc1e1fb3095f2b3, ; 906: lib_System.Net.Http.Json.dll.so => 65
	i64 u0xc040a4ab55817f58, ; 907: ar/Microsoft.Maui.Controls.resources.dll => 360
	i64 u0xc07cadab29efeba0, ; 908: Xamarin.AndroidX.Core.Core.Ktx.dll => 291
	i64 u0xc0d928351ab5ca77, ; 909: System.Console.dll => 20
	i64 u0xc0f5a221a9383aea, ; 910: System.Runtime.Intrinsics => 111
	i64 u0xc111030af54d7191, ; 911: System.Resources.Writer => 103
	i64 u0xc12b8b3afa48329c, ; 912: lib_System.Linq.dll.so => 63
	i64 u0xc1347413e524ff69, ; 913: lib_Syncfusion.Maui.Toolkit.dll.so => 263
	i64 u0xc183ca0b74453aa9, ; 914: lib_System.Threading.Tasks.Dataflow.dll.so => 145
	i64 u0xc1c2cb7af77b8858, ; 915: Microsoft.EntityFrameworkCore => 214
	i64 u0xc1ebdc7e6a943450, ; 916: Microsoft.AspNetCore.Authorization.dll => 192
	i64 u0xc1ff9ae3cdb6e1e6, ; 917: Xamarin.AndroidX.Activity.dll => 272
	i64 u0xc2654c6e949f22d9, ; 918: Microsoft.AspNetCore.Identity.EntityFrameworkCore.dll => 205
	i64 u0xc26c064effb1dea9, ; 919: System.Buffers.dll => 7
	i64 u0xc278de356ad8a9e3, ; 920: Microsoft.IdentityModel.Logging => 243
	i64 u0xc27e35acb993bc55, ; 921: Microsoft.AspNetCore.Identity.dll => 204
	i64 u0xc28c50f32f81cc73, ; 922: ja/Microsoft.Maui.Controls.resources.dll => 375
	i64 u0xc2902f6cf5452577, ; 923: lib_Mono.Android.Export.dll.so => 173
	i64 u0xc2a3bca55b573141, ; 924: System.IO.FileSystem.Watcher => 50
	i64 u0xc2bcfec99f69365e, ; 925: Xamarin.AndroidX.ViewPager2.dll => 343
	i64 u0xc30b52815b58ac2c, ; 926: lib_System.Runtime.Serialization.Xml.dll.so => 117
	i64 u0xc3492f8f90f96ce4, ; 927: lib_Microsoft.Extensions.DependencyModel.dll.so => 225
	i64 u0xc36d7d89c652f455, ; 928: System.Threading.Overlapped => 144
	i64 u0xc36f05e91f701d56, ; 929: lib_Modules.Users.DTO.dll.so => 399
	i64 u0xc374571bc2b0b5f8, ; 930: lib_Microsoft.AspNetCore.Routing.dll.so => 209
	i64 u0xc38fed85a861afa9, ; 931: Microsoft.Extensions.Caching.StackExchangeRedis => 219
	i64 u0xc396b285e59e5493, ; 932: GoogleGson.dll => 184
	i64 u0xc39ced8467203460, ; 933: lib_Refit.HttpClientFactory.dll.so => 256
	i64 u0xc3c86c1e5e12f03d, ; 934: WindowsBase => 169
	i64 u0xc421b61fd853169d, ; 935: lib_System.Net.WebSockets.Client.dll.so => 82
	i64 u0xc4261d083d5c9802, ; 936: Modules.Users.DTO => 399
	i64 u0xc463e077917aa21d, ; 937: System.Runtime.Serialization.Json => 115
	i64 u0xc46e8e6623ac5e82, ; 938: StackExchange.Redis.dll => 262
	i64 u0xc472ce300460ccb6, ; 939: Microsoft.EntityFrameworkCore.dll => 214
	i64 u0xc4d3858ed4d08512, ; 940: Xamarin.AndroidX.Lifecycle.ViewModelSavedState.dll => 317
	i64 u0xc4d69851fe06342f, ; 941: lib_Microsoft.Extensions.Caching.Memory.dll.so => 218
	i64 u0xc50fded0ded1418c, ; 942: lib_System.ComponentModel.TypeConverter.dll.so => 17
	i64 u0xc519125d6bc8fb11, ; 943: lib_System.Net.Requests.dll.so => 74
	i64 u0xc5293b19e4dc230e, ; 944: Xamarin.AndroidX.Navigation.Fragment => 322
	i64 u0xc5325b2fcb37446f, ; 945: lib_System.Private.Xml.dll.so => 91
	i64 u0xc535cb9a21385d9b, ; 946: lib_Xamarin.Android.Glide.DiskLruCache.dll.so => 270
	i64 u0xc5a0f4b95a699af7, ; 947: lib_System.Private.Uri.dll.so => 89
	i64 u0xc5cdcd5b6277579e, ; 948: lib_System.Security.Cryptography.Algorithms.dll.so => 122
	i64 u0xc5ec286825cb0bf4, ; 949: Xamarin.AndroidX.Tracing.Tracing => 336
	i64 u0xc6706bc8aa7fe265, ; 950: Xamarin.AndroidX.Annotation.Jvm => 276
	i64 u0xc74d70d4aa96cef3, ; 951: Xamarin.AndroidX.Navigation.Runtime.Android => 324
	i64 u0xc7c01e7d7c93a110, ; 952: System.Text.Encoding.Extensions.dll => 137
	i64 u0xc7ce851898a4548e, ; 953: lib_System.Web.HttpUtility.dll.so => 156
	i64 u0xc809d4089d2556b2, ; 954: System.Runtime.InteropServices.JavaScript.dll => 108
	i64 u0xc858a28d9ee5a6c5, ; 955: lib_System.Collections.Specialized.dll.so => 11
	i64 u0xc8ac7c6bf1c2ec51, ; 956: System.Reflection.DispatchProxy.dll => 92
	i64 u0xc99ccc413e3ce0d4, ; 957: lib_Microsoft.AspNetCore.Identity.EntityFrameworkCore.dll.so => 205
	i64 u0xc9c62c8f354ac568, ; 958: lib_System.Diagnostics.TextWriterTraceListener.dll.so => 31
	i64 u0xc9e54b32fc19baf3, ; 959: lib_CommunityToolkit.Maui.dll.so => 178
	i64 u0xca32340d8d54dcd5, ; 960: Microsoft.Extensions.Caching.Memory.dll => 218
	i64 u0xca3a723e7342c5b6, ; 961: lib-tr-Microsoft.Maui.Controls.resources.dll.so => 388
	i64 u0xca5801070d9fccfb, ; 962: System.Text.Encoding => 138
	i64 u0xcab3493c70141c2d, ; 963: pl/Microsoft.Maui.Controls.resources => 380
	i64 u0xcacfddc9f7c6de76, ; 964: ro/Microsoft.Maui.Controls.resources.dll => 383
	i64 u0xcadbc92899a777f0, ; 965: Xamarin.AndroidX.Startup.StartupRuntime => 334
	i64 u0xcb45618372c47127, ; 966: Microsoft.EntityFrameworkCore.Relational => 216
	i64 u0xcb6f731cbdfa3dd8, ; 967: Npgsql.EntityFrameworkCore.PostgreSQL => 253
	i64 u0xcba1cb79f45292b5, ; 968: Xamarin.Android.Glide.GifDecoder.dll => 271
	i64 u0xcbb5f80c7293e696, ; 969: lib_System.Globalization.Calendars.dll.so => 40
	i64 u0xcbd4fdd9cef4a294, ; 970: lib__Microsoft.Android.Resource.Designer.dll.so => 401
	i64 u0xcc15da1e07bbd994, ; 971: Xamarin.AndroidX.SlidingPaneLayout => 333
	i64 u0xcc2876b32ef2794c, ; 972: lib_System.Text.RegularExpressions.dll.so => 141
	i64 u0xcc5c3bb714c4561e, ; 973: Xamarin.KotlinX.Coroutines.Core.Jvm.dll => 357
	i64 u0xcc76886e09b88260, ; 974: Xamarin.KotlinX.Serialization.Core.Jvm.dll => 359
	i64 u0xcc9fa2923aa1c9ef, ; 975: System.Diagnostics.Contracts.dll => 25
	i64 u0xccae9bb73e2326bd, ; 976: lib_System.IO.Hashing.dll.so => 176
	i64 u0xccf25c4b634ccd3a, ; 977: zh-Hans/Microsoft.Maui.Controls.resources.dll => 392
	i64 u0xcd10a42808629144, ; 978: System.Net.Requests => 74
	i64 u0xcdca1b920e9f53ba, ; 979: Xamarin.AndroidX.Interpolator => 303
	i64 u0xcdd0c48b6937b21c, ; 980: Xamarin.AndroidX.SwipeRefreshLayout => 335
	i64 u0xcf23d8093f3ceadf, ; 981: System.Diagnostics.DiagnosticSource.dll => 27
	i64 u0xcf5ff6b6b2c4c382, ; 982: System.Net.Mail.dll => 68
	i64 u0xcf8fc898f98b0d34, ; 983: System.Private.Xml.Linq => 90
	i64 u0xcfcfd50998ac9247, ; 984: lib_FluentValidation.dll.so => 182
	i64 u0xd04b5f59ed596e31, ; 985: System.Reflection.Metadata.dll => 97
	i64 u0xd063299fcfc0c93f, ; 986: lib_System.Runtime.Serialization.Json.dll.so => 115
	i64 u0xd0de8a113e976700, ; 987: System.Diagnostics.TextWriterTraceListener => 31
	i64 u0xd0fc33d5ae5d4cb8, ; 988: System.Runtime.Extensions => 106
	i64 u0xd1194e1d8a8de83c, ; 989: lib_Xamarin.AndroidX.Lifecycle.Common.Jvm.dll.so => 305
	i64 u0xd12beacdfc14f696, ; 990: System.Dynamic.Runtime => 37
	i64 u0xd16fd7fb9bbcd43e, ; 991: Microsoft.Extensions.Diagnostics.Abstractions => 227
	i64 u0xd198e7ce1b6a8344, ; 992: System.Net.Quic.dll => 73
	i64 u0xd21d3815977b72a6, ; 993: lib_WorkoutLogg.dll.so => 0
	i64 u0xd3144156a3727ebe, ; 994: Xamarin.Google.Guava.ListenableFuture => 351
	i64 u0xd333d0af9e423810, ; 995: System.Runtime.InteropServices => 110
	i64 u0xd33a415cb4278969, ; 996: System.Security.Cryptography.Encoding.dll => 125
	i64 u0xd3426d966bb704f5, ; 997: Xamarin.AndroidX.AppCompat.AppCompatResources.dll => 278
	i64 u0xd3651b6fc3125825, ; 998: System.Private.Uri.dll => 89
	i64 u0xd373685349b1fe8b, ; 999: Microsoft.Extensions.Logging.dll => 233
	i64 u0xd3801faafafb7698, ; 1000: System.Private.DataContractSerialization.dll => 88
	i64 u0xd3e4c8d6a2d5d470, ; 1001: it/Microsoft.Maui.Controls.resources => 374
	i64 u0xd3edcc1f25459a50, ; 1002: System.Reflection.Emit => 95
	i64 u0xd42655883bb8c19f, ; 1003: Microsoft.EntityFrameworkCore.Abstractions.dll => 215
	i64 u0xd4645626dffec99d, ; 1004: lib_Microsoft.Extensions.DependencyInjection.Abstractions.dll.so => 224
	i64 u0xd4fa0abb79079ea9, ; 1005: System.Security.Principal.dll => 131
	i64 u0xd5507e11a2b2839f, ; 1006: Xamarin.AndroidX.Lifecycle.ViewModelSavedState => 317
	i64 u0xd5d04bef8478ea19, ; 1007: Xamarin.AndroidX.Tracing.Tracing.dll => 336
	i64 u0xd60815f26a12e140, ; 1008: Microsoft.Extensions.Logging.Debug.dll => 235
	i64 u0xd6694f8359737e4e, ; 1009: Xamarin.AndroidX.SavedState => 329
	i64 u0xd6949e129339eae5, ; 1010: lib_Xamarin.AndroidX.Core.Core.Ktx.dll.so => 291
	i64 u0xd6d21782156bc35b, ; 1011: Xamarin.AndroidX.SwipeRefreshLayout.dll => 335
	i64 u0xd6de019f6af72435, ; 1012: Xamarin.AndroidX.ConstraintLayout.Core.dll => 288
	i64 u0xd70956d1e6deefb9, ; 1013: Jsr305Binding => 348
	i64 u0xd72329819cbbbc44, ; 1014: lib_Microsoft.Extensions.Configuration.Abstractions.dll.so => 221
	i64 u0xd72c760af136e863, ; 1015: System.Xml.XmlSerializer.dll => 166
	i64 u0xd753f071e44c2a03, ; 1016: lib_System.Security.SecureString.dll.so => 132
	i64 u0xd7b3764ada9d341d, ; 1017: lib_Microsoft.Extensions.Logging.Abstractions.dll.so => 234
	i64 u0xd7f0088bc5ad71f2, ; 1018: Xamarin.AndroidX.VersionedParcelable => 341
	i64 u0xd824ef6ab33f8f7a, ; 1019: Xamarin.AndroidX.Window.WindowCore.dll => 345
	i64 u0xd8fb25e28ae30a12, ; 1020: Xamarin.AndroidX.ProfileInstaller.ProfileInstaller.dll => 326
	i64 u0xda1dfa4c534a9251, ; 1021: Microsoft.Extensions.DependencyInjection => 223
	i64 u0xdad05a11827959a3, ; 1022: System.Collections.NonGeneric.dll => 10
	i64 u0xdaefdfe71aa53cf9, ; 1023: System.IO.FileSystem.Primitives => 49
	i64 u0xdb5383ab5865c007, ; 1024: lib-vi-Microsoft.Maui.Controls.resources.dll.so => 390
	i64 u0xdb58816721c02a59, ; 1025: lib_System.Reflection.Emit.ILGeneration.dll.so => 93
	i64 u0xdb9f2880a64da6d6, ; 1026: Microsoft.Extensions.Identity.Stores.dll => 232
	i64 u0xdbeda89f832aa805, ; 1027: vi/Microsoft.Maui.Controls.resources.dll => 390
	i64 u0xdbf2a779fbc3ac31, ; 1028: System.Transactions.Local.dll => 153
	i64 u0xdbf9607a441b4505, ; 1029: System.Linq => 63
	i64 u0xdbfc90157a0de9b0, ; 1030: lib_System.Text.Encoding.dll.so => 138
	i64 u0xdc75032002d1a212, ; 1031: lib_System.Transactions.Local.dll.so => 153
	i64 u0xdca8be7403f92d4f, ; 1032: lib_System.Linq.Queryable.dll.so => 62
	i64 u0xdce2c53525640bf3, ; 1033: Microsoft.Extensions.Logging => 233
	i64 u0xdd2b722d78ef5f43, ; 1034: System.Runtime.dll => 119
	i64 u0xdd67031857c72f96, ; 1035: lib_System.Text.Encodings.Web.dll.so => 139
	i64 u0xdd70765ad6162057, ; 1036: Xamarin.JSpecify => 353
	i64 u0xdd92e229ad292030, ; 1037: System.Numerics.dll => 86
	i64 u0xdde30e6b77aa6f6c, ; 1038: lib-zh-Hans-Microsoft.Maui.Controls.resources.dll.so => 392
	i64 u0xde110ae80fa7c2e2, ; 1039: System.Xml.XDocument.dll => 162
	i64 u0xde37b315aaa4d425, ; 1040: Confluent.Kafka.dll => 181
	i64 u0xde4726fcdf63a198, ; 1041: Xamarin.AndroidX.Transition => 338
	i64 u0xde572c2b2fb32f93, ; 1042: lib_System.Threading.Tasks.Extensions.dll.so => 146
	i64 u0xde8769ebda7d8647, ; 1043: hr/Microsoft.Maui.Controls.resources.dll => 371
	i64 u0xdee075f3477ef6be, ; 1044: Xamarin.AndroidX.ExifInterface.dll => 300
	i64 u0xdf4b773de8fb1540, ; 1045: System.Net.dll => 84
	i64 u0xdf9c7682560a9629, ; 1046: System.Net.ServerSentEvents => 76
	i64 u0xdfa254ebb4346068, ; 1047: System.Net.Ping => 71
	i64 u0xdfa4850418b6c99a, ; 1048: Microsoft.AspNetCore.Hosting.Abstractions => 198
	i64 u0xe0142572c095a480, ; 1049: Xamarin.AndroidX.AppCompat.dll => 277
	i64 u0xe020c74e3723dc6f, ; 1050: Syncfusion.Maui.Toolkit.dll => 263
	i64 u0xe021eaa401792a05, ; 1051: System.Text.Encoding.dll => 138
	i64 u0xe02f89350ec78051, ; 1052: Xamarin.AndroidX.CoordinatorLayout.dll => 289
	i64 u0xe0496b9d65ef5474, ; 1053: Xamarin.Android.Glide.DiskLruCache.dll => 270
	i64 u0xe0be470debe77c12, ; 1054: Microsoft.AspNetCore.Cryptography.Internal.dll => 194
	i64 u0xe10b760bb1462e7a, ; 1055: lib_System.Security.Cryptography.Primitives.dll.so => 127
	i64 u0xe192a588d4410686, ; 1056: lib_System.IO.Pipelines.dll.so => 54
	i64 u0xe1a08bd3fa539e0d, ; 1057: System.Runtime.Loader => 112
	i64 u0xe1a77eb8831f7741, ; 1058: System.Security.SecureString.dll => 132
	i64 u0xe1b52f9f816c70ef, ; 1059: System.Private.Xml.Linq.dll => 90
	i64 u0xe1e199c8ab02e356, ; 1060: System.Data.DataSetExtensions.dll => 23
	i64 u0xe1ecfdb7fff86067, ; 1061: System.Net.Security.dll => 75
	i64 u0xe2252a80fe853de4, ; 1062: lib_System.Security.Principal.dll.so => 131
	i64 u0xe22fa4c9c645db62, ; 1063: System.Diagnostics.TextWriterTraceListener.dll => 31
	i64 u0xe24095a7afddaab3, ; 1064: lib_Microsoft.Extensions.Hosting.Abstractions.dll.so => 229
	i64 u0xe2420585aeceb728, ; 1065: System.Net.Requests.dll => 74
	i64 u0xe26692647e6bcb62, ; 1066: Xamarin.AndroidX.Lifecycle.Runtime.Ktx => 312
	i64 u0xe29b73bc11392966, ; 1067: lib-id-Microsoft.Maui.Controls.resources.dll.so => 373
	i64 u0xe2ad448dee50fbdf, ; 1068: System.Xml.Serialization => 161
	i64 u0xe2d920f978f5d85c, ; 1069: System.Data.DataSetExtensions => 23
	i64 u0xe2e426c7714fa0bc, ; 1070: Microsoft.Win32.Primitives.dll => 4
	i64 u0xe332bacb3eb4a806, ; 1071: Mono.Android.Export.dll => 173
	i64 u0xe3811d68d4fe8463, ; 1072: pt-BR/Microsoft.Maui.Controls.resources.dll => 381
	i64 u0xe38e8ac420aab422, ; 1073: Microsoft.AspNetCore.Mvc.Core => 207
	i64 u0xe3a586956771a0ed, ; 1074: lib_SQLite-net.dll.so => 257
	i64 u0xe3b7cbae5ad66c75, ; 1075: lib_System.Security.Cryptography.Encoding.dll.so => 125
	i64 u0xe4292b48f3224d5b, ; 1076: lib_Xamarin.AndroidX.Core.ViewTree.dll.so => 292
	i64 u0xe494f7ced4ecd10a, ; 1077: hu/Microsoft.Maui.Controls.resources.dll => 372
	i64 u0xe4a9b1e40d1e8917, ; 1078: lib-fi-Microsoft.Maui.Controls.resources.dll.so => 367
	i64 u0xe4f74a0b5bf9703f, ; 1079: System.Runtime.Serialization.Primitives => 116
	i64 u0xe525d740098b0a3c, ; 1080: lib_Moduels.Workouts.DTO.dll.so => 394
	i64 u0xe5434e8a119ceb69, ; 1081: lib_Mono.Android.dll.so => 175
	i64 u0xe55703b9ce5c038a, ; 1082: System.Diagnostics.Tools => 32
	i64 u0xe57013c8afc270b5, ; 1083: Microsoft.VisualBasic => 3
	i64 u0xe62913cc36bc07ec, ; 1084: System.Xml.dll => 167
	i64 u0xe66e263beb16318f, ; 1085: Microsoft.Extensions.WebEncoders => 240
	i64 u0xe7bea09c4900a191, ; 1086: Xamarin.AndroidX.VectorDrawable.dll => 339
	i64 u0xe7e03cc18dcdeb49, ; 1087: lib_System.Diagnostics.StackTrace.dll.so => 30
	i64 u0xe7e147ff99a7a380, ; 1088: lib_System.Configuration.dll.so => 19
	i64 u0xe8397cf3948e7cb7, ; 1089: lib_Microsoft.Extensions.Options.ConfigurationExtensions.dll.so => 238
	i64 u0xe86b0df4ba9e5db8, ; 1090: lib_Xamarin.AndroidX.Lifecycle.Runtime.Android.dll.so => 311
	i64 u0xe896622fe0902957, ; 1091: System.Reflection.Emit.dll => 95
	i64 u0xe89a2a9ef110899b, ; 1092: System.Drawing.dll => 36
	i64 u0xe8c35a466559994c, ; 1093: lib_Microsoft.Extensions.Identity.Stores.dll.so => 232
	i64 u0xe8c5f8c100b5934b, ; 1094: Microsoft.Win32.Registry => 5
	i64 u0xe98163eb702ae5c5, ; 1095: Xamarin.AndroidX.Arch.Core.Runtime => 280
	i64 u0xe98b0e4b4d44e931, ; 1096: lib_Grpc.Net.Client.dll.so => 186
	i64 u0xe994f23ba4c143e5, ; 1097: Xamarin.KotlinX.Coroutines.Android => 355
	i64 u0xe9b9c8c0458fd92a, ; 1098: System.Windows => 158
	i64 u0xe9d166d87a7f2bdb, ; 1099: lib_Xamarin.AndroidX.Startup.StartupRuntime.dll.so => 334
	i64 u0xea008206567504c4, ; 1100: Syncfusion.Maui.Toolkit => 263
	i64 u0xea5a4efc2ad81d1b, ; 1101: Xamarin.Google.ErrorProne.Annotations => 350
	i64 u0xeb2313fe9d65b785, ; 1102: Xamarin.AndroidX.ConstraintLayout.dll => 287
	i64 u0xec8abb68d340aac6, ; 1103: Microsoft.AspNetCore.Authorization => 192
	i64 u0xed19c616b3fcb7eb, ; 1104: Xamarin.AndroidX.VersionedParcelable.dll => 341
	i64 u0xedc4817167106c23, ; 1105: System.Net.Sockets.dll => 78
	i64 u0xedc632067fb20ff3, ; 1106: System.Memory.dll => 64
	i64 u0xedc8e4ca71a02a8b, ; 1107: Xamarin.AndroidX.Navigation.Runtime.dll => 323
	i64 u0xee81f5b3f1c4f83b, ; 1108: System.Threading.ThreadPool => 150
	i64 u0xeeb7ebb80150501b, ; 1109: lib_Xamarin.AndroidX.Collection.Jvm.dll.so => 284
	i64 u0xeefc635595ef57f0, ; 1110: System.Security.Cryptography.Cng => 123
	i64 u0xef03b1b5a04e9709, ; 1111: System.Text.Encoding.CodePages.dll => 136
	i64 u0xef5bcbe61622ee5f, ; 1112: Xamarin.AndroidX.Tracing.Tracing.Android.dll => 337
	i64 u0xef602c523fe2e87a, ; 1113: lib_Xamarin.Google.Guava.ListenableFuture.dll.so => 351
	i64 u0xef72742e1bcca27a, ; 1114: Microsoft.Maui.Essentials.dll => 248
	i64 u0xefd1e0c4e5c9b371, ; 1115: System.Resources.ResourceManager.dll => 102
	i64 u0xefe8f8d5ed3c72ea, ; 1116: System.Formats.Tar.dll => 39
	i64 u0xefec0b7fdc57ec42, ; 1117: Xamarin.AndroidX.Activity => 272
	i64 u0xeff59cbde4363ec3, ; 1118: System.Threading.AccessControl.dll => 142
	i64 u0xf00c29406ea45e19, ; 1119: es/Microsoft.Maui.Controls.resources.dll => 366
	i64 u0xf020834425394c93, ; 1120: Microsoft.AspNetCore.ResponseCaching.Abstractions.dll => 208
	i64 u0xf09e47b6ae914f6e, ; 1121: System.Net.NameResolution => 69
	i64 u0xf0ac2b489fed2e35, ; 1122: lib_System.Diagnostics.Debug.dll.so => 26
	i64 u0xf0bb49dadd3a1fe1, ; 1123: lib_System.Net.ServicePoint.dll.so => 77
	i64 u0xf0c16dff90fbf5d6, ; 1124: Xamarin.AndroidX.Window.WindowCore.Jvm => 346
	i64 u0xf0de2537ee19c6ca, ; 1125: lib_System.Net.WebHeaderCollection.dll.so => 80
	i64 u0xf1138779fa181c68, ; 1126: lib_Xamarin.AndroidX.Lifecycle.Runtime.dll.so => 310
	i64 u0xf11b621fc87b983f, ; 1127: Microsoft.Maui.Controls.Xaml.dll => 246
	i64 u0xf161bf2d1e9eaff4, ; 1128: lib_Microsoft.AspNetCore.DataProtection.dll.so => 196
	i64 u0xf161f4f3c3b7e62c, ; 1129: System.Data => 24
	i64 u0xf16eb650d5a464bc, ; 1130: System.ValueTuple => 155
	i64 u0xf1c4b4005493d871, ; 1131: System.Formats.Asn1.dll => 38
	i64 u0xf22514cfad2d598b, ; 1132: lib_Xamarin.AndroidX.Lifecycle.ViewModelSavedState.Android.dll.so => 318
	i64 u0xf238bd79489d3a96, ; 1133: lib-nl-Microsoft.Maui.Controls.resources.dll.so => 379
	i64 u0xf2feea356ba760af, ; 1134: Xamarin.AndroidX.Arch.Core.Runtime.dll => 280
	i64 u0xf300e085f8acd238, ; 1135: lib_System.ServiceProcess.dll.so => 135
	i64 u0xf34e52b26e7e059d, ; 1136: System.Runtime.CompilerServices.VisualC.dll => 105
	i64 u0xf37221fda4ef8830, ; 1137: lib_Xamarin.Google.Android.Material.dll.so => 347
	i64 u0xf3ad9b8fb3eefd12, ; 1138: lib_System.IO.UnmanagedMemoryStream.dll.so => 57
	i64 u0xf3ddfe05336abf29, ; 1139: System => 168
	i64 u0xf408654b2a135055, ; 1140: System.Reflection.Emit.ILGeneration.dll => 93
	i64 u0xf4103170a1de5bd0, ; 1141: System.Linq.Queryable.dll => 62
	i64 u0xf42ad2f4323b64d3, ; 1142: Microsoft.Net.Http.Headers.dll => 250
	i64 u0xf42d20c23173d77c, ; 1143: lib_System.ServiceModel.Web.dll.so => 134
	i64 u0xf4c1dd70a5496a17, ; 1144: System.IO.Compression => 46
	i64 u0xf4ecf4b9afc64781, ; 1145: System.ServiceProcess.dll => 135
	i64 u0xf4eeeaa566e9b970, ; 1146: lib_Xamarin.AndroidX.CustomView.PoolingContainer.dll.so => 295
	i64 u0xf518f63ead11fcd1, ; 1147: System.Threading.Tasks => 148
	i64 u0xf5fc7602fe27b333, ; 1148: System.Net.WebHeaderCollection => 80
	i64 u0xf6077741019d7428, ; 1149: Xamarin.AndroidX.CoordinatorLayout => 289
	i64 u0xf61ade9836ad4692, ; 1150: Microsoft.IdentityModel.Tokens.dll => 244
	i64 u0xf6742cbf457c450b, ; 1151: Xamarin.AndroidX.Lifecycle.Runtime.Android.dll => 311
	i64 u0xf6c0e7d55a7a4e4f, ; 1152: Microsoft.IdentityModel.JsonWebTokens => 242
	i64 u0xf6e8de2aebcbb422, ; 1153: lib_Xamarin.AndroidX.Window.WindowCore.Jvm.dll.so => 346
	i64 u0xf6f893f692f8cb43, ; 1154: Microsoft.Extensions.Options.ConfigurationExtensions.dll => 238
	i64 u0xf70c0a7bf8ccf5af, ; 1155: System.Web => 157
	i64 u0xf77b20923f07c667, ; 1156: de/Microsoft.Maui.Controls.resources.dll => 364
	i64 u0xf79cbf52994c8548, ; 1157: Npgsql => 252
	i64 u0xf7be38c7938ad857, ; 1158: Microsoft.AspNetCore.Cryptography.KeyDerivation => 195
	i64 u0xf7e2cac4c45067b3, ; 1159: lib_System.Numerics.Vectors.dll.so => 85
	i64 u0xf7e74930e0e3d214, ; 1160: zh-HK/Microsoft.Maui.Controls.resources.dll => 391
	i64 u0xf7fa0bf77fe677cc, ; 1161: Newtonsoft.Json.dll => 251
	i64 u0xf84773b5c81e3cef, ; 1162: lib-uk-Microsoft.Maui.Controls.resources.dll.so => 389
	i64 u0xf8aac5ea82de1348, ; 1163: System.Linq.Queryable => 62
	i64 u0xf8b77539b362d3ba, ; 1164: lib_System.Reflection.Primitives.dll.so => 98
	i64 u0xf8e045dc345b2ea3, ; 1165: lib_Xamarin.AndroidX.RecyclerView.dll.so => 327
	i64 u0xf915dc29808193a1, ; 1166: System.Web.HttpUtility.dll => 156
	i64 u0xf96c777a2a0686f4, ; 1167: hi/Microsoft.Maui.Controls.resources.dll => 370
	i64 u0xf9be54c8bcf8ff3b, ; 1168: System.Security.AccessControl.dll => 120
	i64 u0xf9eec5bb3a6aedc6, ; 1169: Microsoft.Extensions.Options => 237
	i64 u0xfa0e82300e67f913, ; 1170: lib_System.AppContext.dll.so => 6
	i64 u0xfa2fdb27e8a2c8e8, ; 1171: System.ComponentModel.EventBasedAsync => 15
	i64 u0xfa3f278f288b0e84, ; 1172: lib_System.Net.Security.dll.so => 75
	i64 u0xfa504dfa0f097d72, ; 1173: Microsoft.Extensions.FileProviders.Abstractions.dll => 228
	i64 u0xfa5ed7226d978949, ; 1174: lib-ar-Microsoft.Maui.Controls.resources.dll.so => 360
	i64 u0xfa645d91e9fc4cba, ; 1175: System.Threading.Thread => 149
	i64 u0xfad4d2c770e827f9, ; 1176: lib_System.IO.IsolatedStorage.dll.so => 52
	i64 u0xfb022853d73b7fa5, ; 1177: lib_SQLitePCLRaw.batteries_v2.dll.so => 258
	i64 u0xfb06dd2338e6f7c4, ; 1178: System.Net.Ping.dll => 71
	i64 u0xfb087abe5365e3b7, ; 1179: lib_System.Data.DataSetExtensions.dll.so => 23
	i64 u0xfb2f5086cd5f5de4, ; 1180: lib_StackExchange.Redis.dll.so => 262
	i64 u0xfb846e949baff5ea, ; 1181: System.Xml.Serialization.dll => 161
	i64 u0xfbad3e4ce4b98145, ; 1182: System.Security.Cryptography.X509Certificates => 128
	i64 u0xfbd71978549ea473, ; 1183: Microsoft.AspNetCore.Http.Features.dll => 203
	i64 u0xfbf0a31c9fc34bc4, ; 1184: lib_System.Net.Http.dll.so => 66
	i64 u0xfc434411e14afaaf, ; 1185: Moduels.Workouts.DTO => 394
	i64 u0xfc6b7527cc280b3f, ; 1186: lib_System.Runtime.Serialization.Formatters.dll.so => 114
	i64 u0xfc719aec26adf9d9, ; 1187: Xamarin.AndroidX.Navigation.Fragment.dll => 322
	i64 u0xfc82690c2fe2735c, ; 1188: Xamarin.AndroidX.Lifecycle.Process.dll => 309
	i64 u0xfc93fc307d279893, ; 1189: System.IO.Pipes.AccessControl.dll => 55
	i64 u0xfcd302092ada6328, ; 1190: System.IO.MemoryMappedFiles.dll => 53
	i64 u0xfd22f00870e40ae0, ; 1191: lib_Xamarin.AndroidX.DrawerLayout.dll.so => 296
	i64 u0xfd49b3c1a76e2748, ; 1192: System.Runtime.InteropServices.RuntimeInformation => 109
	i64 u0xfd536c702f64dc47, ; 1193: System.Text.Encoding.Extensions => 137
	i64 u0xfd583f7657b6a1cb, ; 1194: Xamarin.AndroidX.Fragment => 301
	i64 u0xfd8dd91a2c26bd5d, ; 1195: Xamarin.AndroidX.Lifecycle.Runtime => 310
	i64 u0xfda36abccf05cf5c, ; 1196: System.Net.WebSockets.Client => 82
	i64 u0xfdbe4710aa9beeff, ; 1197: CommunityToolkit.Maui => 178
	i64 u0xfddbe9695626a7f5, ; 1198: Xamarin.AndroidX.Lifecycle.Common => 304
	i64 u0xfeae9952cf03b8cb, ; 1199: tr/Microsoft.Maui.Controls.resources => 388
	i64 u0xfebe1950717515f9, ; 1200: Xamarin.AndroidX.Lifecycle.LiveData.Core.Ktx.dll => 308
	i64 u0xff1a4e86e72b0140, ; 1201: Microsoft.AspNetCore.Authentication.Abstractions.dll => 189
	i64 u0xff270a55858bac8d, ; 1202: System.Security.Principal => 131
	i64 u0xff9b54613e0d2cc8, ; 1203: System.Net.Http.Json => 65
	i64 u0xffd5b3e75321a00b, ; 1204: Microsoft.AspNetCore.DataProtection.Abstractions => 197
	i64 u0xffdb7a971be4ec73 ; 1205: System.ValueTuple.dll => 155
], align 16

@assembly_image_cache_indices = dso_local local_unnamed_addr constant [1206 x i32] [
	i32 42, i32 356, i32 335, i32 255, i32 13, i32 193, i32 186, i32 323,
	i32 179, i32 238, i32 266, i32 107, i32 218, i32 174, i32 48, i32 277,
	i32 7, i32 261, i32 197, i32 88, i32 384, i32 362, i32 390, i32 241,
	i32 297, i32 72, i32 327, i32 212, i32 12, i32 247, i32 104, i32 199,
	i32 212, i32 391, i32 159, i32 19, i32 302, i32 284, i32 164, i32 299,
	i32 240, i32 339, i32 170, i32 384, i32 10, i32 235, i32 340, i32 98,
	i32 295, i32 296, i32 13, i32 237, i32 10, i32 129, i32 197, i32 97,
	i32 217, i32 267, i32 143, i32 39, i32 385, i32 359, i32 195, i32 342,
	i32 381, i32 175, i32 271, i32 5, i32 248, i32 68, i32 332, i32 132,
	i32 231, i32 185, i32 331, i32 298, i32 69, i32 285, i32 67, i32 198,
	i32 57, i32 294, i32 52, i32 43, i32 236, i32 127, i32 68, i32 83,
	i32 312, i32 161, i32 94, i32 101, i32 327, i32 144, i32 154, i32 281,
	i32 368, i32 165, i32 172, i32 369, i32 224, i32 83, i32 353, i32 285,
	i32 4, i32 5, i32 51, i32 103, i32 225, i32 56, i32 122, i32 100,
	i32 171, i32 120, i32 356, i32 21, i32 253, i32 372, i32 139, i32 395,
	i32 99, i32 359, i32 79, i32 378, i32 256, i32 396, i32 334, i32 121,
	i32 199, i32 8, i32 168, i32 387, i32 71, i32 270, i32 313, i32 328,
	i32 202, i32 226, i32 174, i32 148, i32 40, i32 332, i32 47, i32 183,
	i32 30, i32 325, i32 376, i32 147, i32 237, i32 166, i32 231, i32 28,
	i32 86, i32 336, i32 207, i32 79, i32 43, i32 29, i32 42, i32 105,
	i32 119, i32 275, i32 45, i32 93, i32 387, i32 56, i32 151, i32 149,
	i32 214, i32 102, i32 49, i32 20, i32 290, i32 116, i32 268, i32 368,
	i32 349, i32 258, i32 354, i32 239, i32 96, i32 58, i32 265, i32 373,
	i32 371, i32 345, i32 83, i32 349, i32 172, i32 26, i32 72, i32 326,
	i32 255, i32 211, i32 227, i32 300, i32 321, i32 389, i32 70, i32 33,
	i32 367, i32 14, i32 141, i32 265, i32 38, i32 393, i32 286, i32 211,
	i32 380, i32 400, i32 136, i32 94, i32 90, i32 152, i32 346, i32 386,
	i32 24, i32 140, i32 57, i32 142, i32 394, i32 51, i32 365, i32 262,
	i32 29, i32 160, i32 34, i32 167, i32 217, i32 230, i32 182, i32 301,
	i32 241, i32 52, i32 401, i32 344, i32 92, i32 282, i32 35, i32 368,
	i32 160, i32 9, i32 366, i32 78, i32 59, i32 55, i32 247, i32 362,
	i32 245, i32 13, i32 343, i32 220, i32 279, i32 111, i32 188, i32 316,
	i32 252, i32 32, i32 106, i32 86, i32 94, i32 53, i32 98, i32 188,
	i32 352, i32 58, i32 194, i32 9, i32 104, i32 294, i32 69, i32 193,
	i32 342, i32 361, i32 251, i32 228, i32 127, i32 328, i32 118, i32 137,
	i32 330, i32 244, i32 128, i32 108, i32 354, i32 133, i32 202, i32 281,
	i32 351, i32 150, i32 159, i32 302, i32 290, i32 258, i32 183, i32 297,
	i32 328, i32 99, i32 200, i32 24, i32 333, i32 146, i32 229, i32 320,
	i32 3, i32 219, i32 170, i32 278, i32 102, i32 164, i32 101, i32 292,
	i32 190, i32 25, i32 204, i32 95, i32 171, i32 175, i32 273, i32 3,
	i32 380, i32 337, i32 299, i32 1, i32 116, i32 354, i32 191, i32 215,
	i32 302, i32 309, i32 265, i32 33, i32 6, i32 225, i32 267, i32 384,
	i32 159, i32 187, i32 382, i32 53, i32 240, i32 87, i32 250, i32 341,
	i32 325, i32 44, i32 308, i32 106, i32 47, i32 208, i32 140, i32 206,
	i32 318, i32 65, i32 216, i32 319, i32 70, i32 82, i32 60, i32 91,
	i32 157, i32 279, i32 135, i32 112, i32 59, i32 374, i32 319, i32 326,
	i32 174, i32 232, i32 136, i32 143, i32 40, i32 361, i32 260, i32 222,
	i32 207, i32 244, i32 245, i32 187, i32 61, i32 180, i32 222, i32 315,
	i32 81, i32 25, i32 36, i32 189, i32 101, i32 312, i32 72, i32 201,
	i32 187, i32 22, i32 290, i32 249, i32 194, i32 385, i32 123, i32 70,
	i32 109, i32 391, i32 213, i32 121, i32 119, i32 304, i32 196, i32 321,
	i32 305, i32 11, i32 2, i32 126, i32 117, i32 145, i32 41, i32 89,
	i32 274, i32 259, i32 177, i32 229, i32 27, i32 151, i32 222, i32 375,
	i32 223, i32 250, i32 350, i32 273, i32 1, i32 199, i32 275, i32 44,
	i32 289, i32 152, i32 18, i32 0, i32 88, i32 363, i32 41, i32 308,
	i32 283, i32 206, i32 313, i32 96, i32 233, i32 28, i32 0, i32 41,
	i32 209, i32 80, i32 298, i32 286, i32 147, i32 110, i32 284, i32 11,
	i32 107, i32 139, i32 16, i32 124, i32 67, i32 160, i32 22, i32 260,
	i32 365, i32 358, i32 104, i32 236, i32 223, i32 357, i32 64, i32 58,
	i32 246, i32 364, i32 112, i32 177, i32 324, i32 355, i32 9, i32 347,
	i32 122, i32 100, i32 107, i32 76, i32 400, i32 316, i32 178, i32 255,
	i32 245, i32 113, i32 276, i32 49, i32 59, i32 20, i32 315, i32 293,
	i32 73, i32 288, i32 158, i32 39, i32 363, i32 35, i32 38, i32 369,
	i32 260, i32 110, i32 378, i32 21, i32 181, i32 352, i32 314, i32 249,
	i32 15, i32 239, i32 81, i32 81, i32 293, i32 239, i32 398, i32 322,
	i32 331, i32 155, i32 21, i32 247, i32 362, i32 50, i32 51, i32 388,
	i32 378, i32 96, i32 269, i32 227, i32 374, i32 16, i32 266, i32 292,
	i32 125, i32 371, i32 163, i32 45, i32 350, i32 184, i32 118, i32 64,
	i32 169, i32 205, i32 226, i32 220, i32 14, i32 329, i32 113, i32 276,
	i32 61, i32 76, i32 123, i32 377, i32 2, i32 387, i32 301, i32 314,
	i32 230, i32 353, i32 314, i32 6, i32 283, i32 367, i32 297, i32 212,
	i32 242, i32 17, i32 385, i32 364, i32 79, i32 287, i32 321, i32 254,
	i32 179, i32 133, i32 352, i32 254, i32 377, i32 396, i32 85, i32 235,
	i32 12, i32 34, i32 121, i32 267, i32 358, i32 309, i32 176, i32 299,
	i32 87, i32 268, i32 195, i32 18, i32 203, i32 342, i32 221, i32 307,
	i32 193, i32 73, i32 97, i32 261, i32 168, i32 303, i32 84, i32 393,
	i32 236, i32 398, i32 277, i32 282, i32 157, i32 36, i32 154, i32 389,
	i32 257, i32 241, i32 392, i32 147, i32 56, i32 115, i32 216, i32 200,
	i32 283, i32 339, i32 338, i32 37, i32 393, i32 220, i32 117, i32 275,
	i32 14, i32 269, i32 215, i32 149, i32 201, i32 43, i32 186, i32 248,
	i32 273, i32 100, i32 357, i32 181, i32 171, i32 16, i32 330, i32 204,
	i32 48, i32 109, i32 202, i32 203, i32 99, i32 217, i32 319, i32 400,
	i32 27, i32 130, i32 29, i32 369, i32 191, i32 228, i32 331, i32 130,
	i32 44, i32 293, i32 298, i32 152, i32 8, i32 198, i32 185, i32 264,
	i32 337, i32 251, i32 320, i32 370, i32 383, i32 259, i32 382, i32 134,
	i32 381, i32 209, i32 42, i32 358, i32 259, i32 33, i32 210, i32 401,
	i32 46, i32 146, i32 315, i32 246, i32 306, i32 294, i32 140, i32 63,
	i32 134, i32 361, i32 48, i32 163, i32 280, i32 264, i32 306, i32 269,
	i32 304, i32 377, i32 338, i32 398, i32 46, i32 167, i32 243, i32 176,
	i32 303, i32 243, i32 256, i32 366, i32 300, i32 373, i32 249, i32 18,
	i32 8, i32 184, i32 291, i32 396, i32 330, i32 126, i32 60, i32 144,
	i32 180, i32 323, i32 376, i32 310, i32 348, i32 252, i32 344, i32 153,
	i32 145, i32 356, i32 128, i32 355, i32 395, i32 163, i32 165, i32 295,
	i32 272, i32 221, i32 324, i32 379, i32 26, i32 320, i32 307, i32 399,
	i32 84, i32 219, i32 344, i32 129, i32 349, i32 103, i32 151, i32 182,
	i32 347, i32 345, i32 325, i32 253, i32 54, i32 165, i32 170, i32 133,
	i32 266, i32 37, i32 340, i32 376, i32 210, i32 226, i32 180, i32 22,
	i32 114, i32 92, i32 50, i32 61, i32 124, i32 85, i32 129, i32 213,
	i32 166, i32 348, i32 169, i32 329, i32 332, i32 296, i32 268, i32 311,
	i32 201, i32 4, i32 305, i32 372, i32 173, i32 2, i32 316, i32 118,
	i32 397, i32 242, i32 274, i32 19, i32 179, i32 234, i32 91, i32 66,
	i32 30, i32 224, i32 365, i32 288, i32 60, i32 206, i32 113, i32 307,
	i32 32, i32 183, i32 130, i32 162, i32 383, i32 190, i32 286, i32 143,
	i32 379, i32 156, i32 17, i32 285, i32 271, i32 77, i32 75, i32 15,
	i32 172, i32 87, i32 257, i32 264, i32 126, i32 306, i32 317, i32 287,
	i32 386, i32 313, i32 34, i32 231, i32 120, i32 141, i32 124, i32 108,
	i32 363, i32 340, i32 282, i32 370, i32 360, i32 397, i32 54, i32 47,
	i32 28, i32 213, i32 191, i32 142, i32 211, i32 148, i32 234, i32 190,
	i32 210, i32 150, i32 35, i32 386, i32 395, i32 177, i32 230, i32 77,
	i32 164, i32 185, i32 1, i32 196, i32 318, i32 333, i32 382, i32 375,
	i32 162, i32 12, i32 158, i32 154, i32 200, i32 78, i32 105, i32 114,
	i32 397, i32 261, i32 279, i32 192, i32 66, i32 67, i32 343, i32 45,
	i32 281, i32 254, i32 111, i32 189, i32 208, i32 7, i32 278, i32 55,
	i32 274, i32 188, i32 65, i32 360, i32 291, i32 20, i32 111, i32 103,
	i32 63, i32 263, i32 145, i32 214, i32 192, i32 272, i32 205, i32 7,
	i32 243, i32 204, i32 375, i32 173, i32 50, i32 343, i32 117, i32 225,
	i32 144, i32 399, i32 209, i32 219, i32 184, i32 256, i32 169, i32 82,
	i32 399, i32 115, i32 262, i32 214, i32 317, i32 218, i32 17, i32 74,
	i32 322, i32 91, i32 270, i32 89, i32 122, i32 336, i32 276, i32 324,
	i32 137, i32 156, i32 108, i32 11, i32 92, i32 205, i32 31, i32 178,
	i32 218, i32 388, i32 138, i32 380, i32 383, i32 334, i32 216, i32 253,
	i32 271, i32 40, i32 401, i32 333, i32 141, i32 357, i32 359, i32 25,
	i32 176, i32 392, i32 74, i32 303, i32 335, i32 27, i32 68, i32 90,
	i32 182, i32 97, i32 115, i32 31, i32 106, i32 305, i32 37, i32 227,
	i32 73, i32 0, i32 351, i32 110, i32 125, i32 278, i32 89, i32 233,
	i32 88, i32 374, i32 95, i32 215, i32 224, i32 131, i32 317, i32 336,
	i32 235, i32 329, i32 291, i32 335, i32 288, i32 348, i32 221, i32 166,
	i32 132, i32 234, i32 341, i32 345, i32 326, i32 223, i32 10, i32 49,
	i32 390, i32 93, i32 232, i32 390, i32 153, i32 63, i32 138, i32 153,
	i32 62, i32 233, i32 119, i32 139, i32 353, i32 86, i32 392, i32 162,
	i32 181, i32 338, i32 146, i32 371, i32 300, i32 84, i32 76, i32 71,
	i32 198, i32 277, i32 263, i32 138, i32 289, i32 270, i32 194, i32 127,
	i32 54, i32 112, i32 132, i32 90, i32 23, i32 75, i32 131, i32 31,
	i32 229, i32 74, i32 312, i32 373, i32 161, i32 23, i32 4, i32 173,
	i32 381, i32 207, i32 257, i32 125, i32 292, i32 372, i32 367, i32 116,
	i32 394, i32 175, i32 32, i32 3, i32 167, i32 240, i32 339, i32 30,
	i32 19, i32 238, i32 311, i32 95, i32 36, i32 232, i32 5, i32 280,
	i32 186, i32 355, i32 158, i32 334, i32 263, i32 350, i32 287, i32 192,
	i32 341, i32 78, i32 64, i32 323, i32 150, i32 284, i32 123, i32 136,
	i32 337, i32 351, i32 248, i32 102, i32 39, i32 272, i32 142, i32 366,
	i32 208, i32 69, i32 26, i32 77, i32 346, i32 80, i32 310, i32 246,
	i32 196, i32 24, i32 155, i32 38, i32 318, i32 379, i32 280, i32 135,
	i32 105, i32 347, i32 57, i32 168, i32 93, i32 62, i32 250, i32 134,
	i32 46, i32 135, i32 295, i32 148, i32 80, i32 289, i32 244, i32 311,
	i32 242, i32 346, i32 238, i32 157, i32 364, i32 252, i32 195, i32 85,
	i32 391, i32 251, i32 389, i32 62, i32 98, i32 327, i32 156, i32 370,
	i32 120, i32 237, i32 6, i32 15, i32 75, i32 228, i32 360, i32 149,
	i32 52, i32 258, i32 71, i32 23, i32 262, i32 161, i32 128, i32 203,
	i32 66, i32 394, i32 114, i32 322, i32 309, i32 55, i32 53, i32 296,
	i32 109, i32 137, i32 301, i32 310, i32 82, i32 178, i32 304, i32 388,
	i32 308, i32 189, i32 131, i32 65, i32 197, i32 155
], align 16

@marshal_methods_number_of_classes = dso_local local_unnamed_addr constant i32 0, align 4

@marshal_methods_class_cache = dso_local local_unnamed_addr global [0 x %struct.MarshalMethodsManagedClass] zeroinitializer, align 8

; Names of classes in which marshal methods reside
@mm_class_names = dso_local local_unnamed_addr constant [0 x ptr] zeroinitializer, align 8

@mm_method_names = dso_local local_unnamed_addr constant [1 x %struct.MarshalMethodName] [
	%struct.MarshalMethodName {
		i64 u0x0000000000000000, ; name: 
		ptr @.MarshalMethodName.0_name; char* name
	} ; 0
], align 8

; get_function_pointer (uint32_t mono_image_index, uint32_t class_index, uint32_t method_token, void*& target_ptr)
@get_function_pointer = internal dso_local unnamed_addr global ptr null, align 8

; Functions

; Function attributes: memory(write, argmem: none, inaccessiblemem: none) "min-legal-vector-width"="0" mustprogress nofree norecurse nosync "no-trapping-math"="true" nounwind "stack-protector-buffer-size"="8" uwtable willreturn
define void @xamarin_app_init(ptr nocapture noundef readnone %env, ptr noundef %fn) local_unnamed_addr #0
{
	%fnIsNull = icmp eq ptr %fn, null
	br i1 %fnIsNull, label %1, label %2

1: ; preds = %0
	%putsResult = call noundef i32 @puts(ptr @.mm.0)
	call void @abort()
	unreachable 

2: ; preds = %1, %0
	store ptr %fn, ptr @get_function_pointer, align 8, !tbaa !3
	ret void
}

; Strings
@.mm.0 = private unnamed_addr constant [40 x i8] c"get_function_pointer MUST be specified\0A\00", align 16

;MarshalMethodName
@.MarshalMethodName.0_name = private unnamed_addr constant [1 x i8] c"\00", align 1

; External functions

; Function attributes: noreturn "no-trapping-math"="true" nounwind "stack-protector-buffer-size"="8"
declare void @abort() local_unnamed_addr #2

; Function attributes: nofree nounwind
declare noundef i32 @puts(ptr noundef) local_unnamed_addr #1
attributes #0 = { memory(write, argmem: none, inaccessiblemem: none) "min-legal-vector-width"="0" mustprogress nofree norecurse nosync "no-trapping-math"="true" nounwind "stack-protector-buffer-size"="8" "target-cpu"="x86-64" "target-features"="+crc32,+cx16,+cx8,+fxsr,+mmx,+popcnt,+sse,+sse2,+sse3,+sse4.1,+sse4.2,+ssse3,+x87" "tune-cpu"="generic" uwtable willreturn }
attributes #1 = { nofree nounwind }
attributes #2 = { noreturn "no-trapping-math"="true" nounwind "stack-protector-buffer-size"="8" "target-cpu"="x86-64" "target-features"="+crc32,+cx16,+cx8,+fxsr,+mmx,+popcnt,+sse,+sse2,+sse3,+sse4.1,+sse4.2,+ssse3,+x87" "tune-cpu"="generic" }

; Metadata
!llvm.module.flags = !{!0, !1}
!0 = !{i32 1, !"wchar_size", i32 4}
!1 = !{i32 7, !"PIC Level", i32 2}
!llvm.ident = !{!2}
!2 = !{!".NET for Android remotes/origin/release/10.0.1xx @ 350a375fc202f0072ac4191624986d8c642b93fa"}
!3 = !{!4, !4, i64 0}
!4 = !{!"any pointer", !5, i64 0}
!5 = !{!"omnipotent char", !6, i64 0}
!6 = !{!"Simple C++ TBAA"}
