; ModuleID = 'marshal_methods.arm64-v8a.ll'
source_filename = "marshal_methods.arm64-v8a.ll"
target datalayout = "e-m:e-i8:8:32-i16:16:32-i64:64-i128:128-n32:64-S128"
target triple = "aarch64-unknown-linux-android21"

%struct.MarshalMethodName = type {
	i64, ; uint64_t id
	ptr ; char* name
}

%struct.MarshalMethodsManagedClass = type {
	i32, ; uint32_t token
	ptr ; MonoClass klass
}

@assembly_image_cache = dso_local local_unnamed_addr global [404 x ptr] zeroinitializer, align 8

; Each entry maps hash of an assembly name to an index into the `assembly_image_cache` array
@assembly_image_cache_hashes = dso_local local_unnamed_addr constant [1212 x i64] [
	i64 u0x001e58127c546039, ; 0: lib_System.Globalization.dll.so => 42
	i64 u0x0024d0f62dee05bd, ; 1: Xamarin.KotlinX.Coroutines.Core.dll => 357
	i64 u0x0071cf2d27b7d61e, ; 2: lib_Xamarin.AndroidX.SwipeRefreshLayout.dll.so => 336
	i64 u0x00b3aadb3a4c4038, ; 3: lib_Refit.dll.so => 256
	i64 u0x01109b0e4d99e61f, ; 4: System.ComponentModel.Annotations.dll => 13
	i64 u0x014b43fdeb5d21ad, ; 5: Microsoft.AspNetCore.Authorization.Policy.dll => 194
	i64 u0x020f428300334897, ; 6: Grpc.Net.Client.dll => 187
	i64 u0x02123411c4e01926, ; 7: lib_Xamarin.AndroidX.Navigation.Runtime.dll.so => 324
	i64 u0x022e81ea9c46e03a, ; 8: lib_CommunityToolkit.Maui.Core.dll.so => 180
	i64 u0x022f31be406de945, ; 9: Microsoft.Extensions.Options.ConfigurationExtensions => 239
	i64 u0x02827b47e97f2378, ; 10: System.Security.Cryptography.Pkcs.dll => 267
	i64 u0x0284512fad379f7e, ; 11: System.Runtime.Handles => 107
	i64 u0x02a4c5a44384f885, ; 12: Microsoft.Extensions.Caching.Memory => 219
	i64 u0x02abedc11addc1ed, ; 13: lib_Mono.Android.Runtime.dll.so => 174
	i64 u0x02f55bf70672f5c8, ; 14: lib_System.IO.FileSystem.DriveInfo.dll.so => 48
	i64 u0x032267b2a94db371, ; 15: lib_Xamarin.AndroidX.AppCompat.dll.so => 278
	i64 u0x03621c804933a890, ; 16: System.Buffers => 7
	i64 u0x0363ac97a4cb84e6, ; 17: SQLitePCLRaw.provider.e_sqlite3.dll => 262
	i64 u0x0377283fc1d7573a, ; 18: Microsoft.AspNetCore.DataProtection.Abstractions.dll => 198
	i64 u0x0399610510a38a38, ; 19: lib_System.Private.DataContractSerialization.dll.so => 88
	i64 u0x043032f1d071fae0, ; 20: ru/Microsoft.Maui.Controls.resources => 386
	i64 u0x044440a55165631e, ; 21: lib-cs-Microsoft.Maui.Controls.resources.dll.so => 364
	i64 u0x046eb1581a80c6b0, ; 22: vi/Microsoft.Maui.Controls.resources => 392
	i64 u0x0470607fd33c32db, ; 23: Microsoft.IdentityModel.Abstractions.dll => 242
	i64 u0x047408741db2431a, ; 24: Xamarin.AndroidX.DynamicAnimation => 298
	i64 u0x0517ef04e06e9f76, ; 25: System.Net.Primitives => 72
	i64 u0x0565d18c6da3de38, ; 26: Xamarin.AndroidX.RecyclerView => 328
	i64 u0x057bf9fa9fb09f7c, ; 27: Microsoft.Data.Sqlite.dll => 213
	i64 u0x0581db89237110e9, ; 28: lib_System.Collections.dll.so => 12
	i64 u0x05989cb940b225a9, ; 29: Microsoft.Maui.dll => 248
	i64 u0x05a1c25e78e22d87, ; 30: lib_System.Runtime.CompilerServices.Unsafe.dll.so => 104
	i64 u0x05c27cf2b380bbf2, ; 31: lib_Microsoft.AspNetCore.Hosting.Server.Abstractions.dll.so => 200
	i64 u0x05ef98b6a1db882c, ; 32: lib_Microsoft.Data.Sqlite.dll.so => 213
	i64 u0x06076b5d2b581f08, ; 33: zh-HK/Microsoft.Maui.Controls.resources => 393
	i64 u0x06388ffe9f6c161a, ; 34: System.Xml.Linq.dll => 159
	i64 u0x06600c4c124cb358, ; 35: System.Configuration.dll => 19
	i64 u0x067f95c5ddab55b3, ; 36: lib_Xamarin.AndroidX.Fragment.Ktx.dll.so => 303
	i64 u0x0680a433c781bb3d, ; 37: Xamarin.AndroidX.Collection.Jvm => 285
	i64 u0x069fff96ec92a91d, ; 38: System.Xml.XPath.dll => 164
	i64 u0x070b0847e18dab68, ; 39: Xamarin.AndroidX.Emoji2.ViewsHelper.dll => 300
	i64 u0x072496def57d8011, ; 40: Microsoft.Extensions.WebEncoders.dll => 241
	i64 u0x0739448d84d3b016, ; 41: lib_Xamarin.AndroidX.VectorDrawable.dll.so => 340
	i64 u0x07469f2eecce9e85, ; 42: mscorlib.dll => 170
	i64 u0x07c57877c7ba78ad, ; 43: ru/Microsoft.Maui.Controls.resources.dll => 386
	i64 u0x07dcdc7460a0c5e4, ; 44: System.Collections.NonGeneric => 10
	i64 u0x08122e52765333c8, ; 45: lib_Microsoft.Extensions.Logging.Debug.dll.so => 236
	i64 u0x088610fc2509f69e, ; 46: lib_Xamarin.AndroidX.VectorDrawable.Animated.dll.so => 341
	i64 u0x08a7c865576bbde7, ; 47: System.Reflection.Primitives => 98
	i64 u0x08c9d051a4a817e5, ; 48: Xamarin.AndroidX.CustomView.PoolingContainer.dll => 296
	i64 u0x08f3c9788ee2153c, ; 49: Xamarin.AndroidX.DrawerLayout => 297
	i64 u0x09138715c92dba90, ; 50: lib_System.ComponentModel.Annotations.dll.so => 13
	i64 u0x0919c28b89381a0b, ; 51: lib_Microsoft.Extensions.Options.dll.so => 238
	i64 u0x092266563089ae3e, ; 52: lib_System.Collections.NonGeneric.dll.so => 10
	i64 u0x09d144a7e214d457, ; 53: System.Security.Cryptography => 129
	i64 u0x09d931c8a4087ae3, ; 54: lib_Microsoft.AspNetCore.DataProtection.Abstractions.dll.so => 198
	i64 u0x09e2b9f743db21a8, ; 55: lib_System.Reflection.Metadata.dll.so => 97
	i64 u0x0a805f95d98f597b, ; 56: lib_Microsoft.Extensions.Caching.Abstractions.dll.so => 218
	i64 u0x0a980941fa112bc4, ; 57: System.Security.Cryptography.Xml => 268
	i64 u0x0abb3e2b271edc45, ; 58: System.Threading.Channels.dll => 143
	i64 u0x0b06b1feab070143, ; 59: System.Formats.Tar => 39
	i64 u0x0b3b632c3bbee20c, ; 60: sk/Microsoft.Maui.Controls.resources => 387
	i64 u0x0b6aff547b84fbe9, ; 61: Xamarin.KotlinX.Serialization.Core.Jvm => 360
	i64 u0x0be1e582d0d8ef1a, ; 62: lib_Microsoft.AspNetCore.Cryptography.KeyDerivation.dll.so => 196
	i64 u0x0be2e1f8ce4064ed, ; 63: Xamarin.AndroidX.ViewPager => 343
	i64 u0x0c3ca6cc978e2aae, ; 64: pt-BR/Microsoft.Maui.Controls.resources => 383
	i64 u0x0c59ad9fbbd43abe, ; 65: Mono.Android => 175
	i64 u0x0c65741e86371ee3, ; 66: lib_Xamarin.Android.Glide.GifDecoder.dll.so => 272
	i64 u0x0c74af560004e816, ; 67: Microsoft.Win32.Registry.dll => 5
	i64 u0x0c7790f60165fc06, ; 68: lib_Microsoft.Maui.Essentials.dll.so => 249
	i64 u0x0c83c82812e96127, ; 69: lib_System.Net.Mail.dll.so => 68
	i64 u0x0cce4bce83380b7f, ; 70: Xamarin.AndroidX.Security.SecurityCrypto => 333
	i64 u0x0d13cd7cce4284e4, ; 71: System.Security.SecureString => 132
	i64 u0x0d34fb076d8103ae, ; 72: Microsoft.Extensions.Identity.Core.dll => 232
	i64 u0x0d565cb22b8879da, ; 73: lib_Grpc.Core.Api.dll.so => 186
	i64 u0x0d63f4f73521c24f, ; 74: lib_Xamarin.AndroidX.SavedState.SavedState.Ktx.dll.so => 332
	i64 u0x0e04e702012f8463, ; 75: Xamarin.AndroidX.Emoji2 => 299
	i64 u0x0e14e73a54dda68e, ; 76: lib_System.Net.NameResolution.dll.so => 69
	i64 u0x0f37dd7a62ae99af, ; 77: lib_Xamarin.AndroidX.Collection.Ktx.dll.so => 286
	i64 u0x0f5e7abaa7cf470a, ; 78: System.Net.HttpListener => 67
	i64 u0x0f948418e9ebd6de, ; 79: Microsoft.AspNetCore.Hosting.Abstractions.dll => 199
	i64 u0x1001f97bbe242e64, ; 80: System.IO.UnmanagedMemoryStream => 57
	i64 u0x102a31b45304b1da, ; 81: Xamarin.AndroidX.CustomView => 295
	i64 u0x1065c4cb554c3d75, ; 82: System.IO.IsolatedStorage.dll => 52
	i64 u0x10f6cfcbcf801616, ; 83: System.IO.Compression.Brotli => 43
	i64 u0x1140109eb2e77ceb, ; 84: Microsoft.Extensions.ObjectPool.dll => 237
	i64 u0x114443cdcf2091f1, ; 85: System.Security.Cryptography.Primitives => 127
	i64 u0x11a603952763e1d4, ; 86: System.Net.Mail => 68
	i64 u0x11a70d0e1009fb11, ; 87: System.Net.WebSockets.dll => 83
	i64 u0x11f26371eee0d3c1, ; 88: lib_Xamarin.AndroidX.Lifecycle.Runtime.Ktx.dll.so => 313
	i64 u0x12128b3f59302d47, ; 89: lib_System.Xml.Serialization.dll.so => 161
	i64 u0x123639456fb056da, ; 90: System.Reflection.Emit.Lightweight.dll => 94
	i64 u0x12521e9764603eaa, ; 91: lib_System.Resources.Reader.dll.so => 101
	i64 u0x125b7f94acb989db, ; 92: Xamarin.AndroidX.RecyclerView.dll => 328
	i64 u0x12d3b63863d4ab0b, ; 93: lib_System.Threading.Overlapped.dll.so => 144
	i64 u0x134eab1061c395ee, ; 94: System.Transactions => 154
	i64 u0x138567fa954faa55, ; 95: Xamarin.AndroidX.Browser => 282
	i64 u0x13a01de0cbc3f06c, ; 96: lib-fr-Microsoft.Maui.Controls.resources.dll.so => 370
	i64 u0x13beedefb0e28a45, ; 97: lib_System.Xml.XmlDocument.dll.so => 165
	i64 u0x13f1e5e209e91af4, ; 98: lib_Java.Interop.dll.so => 172
	i64 u0x13f1e880c25d96d1, ; 99: he/Microsoft.Maui.Controls.resources => 371
	i64 u0x143d8ea60a6a4011, ; 100: Microsoft.Extensions.DependencyInjection.Abstractions => 225
	i64 u0x1497051b917530bd, ; 101: lib_System.Net.WebSockets.dll.so => 83
	i64 u0x14d612a531c79c05, ; 102: Xamarin.JSpecify.dll => 354
	i64 u0x14e68447938213b7, ; 103: Xamarin.AndroidX.Collection.Ktx.dll => 286
	i64 u0x152a448bd1e745a7, ; 104: Microsoft.Win32.Primitives => 4
	i64 u0x1557de0138c445f4, ; 105: lib_Microsoft.Win32.Registry.dll.so => 5
	i64 u0x15bdc156ed462f2f, ; 106: lib_System.IO.FileSystem.dll.so => 51
	i64 u0x15e300c2c1668655, ; 107: System.Resources.Writer.dll => 103
	i64 u0x16054fdcb6b3098b, ; 108: Microsoft.Extensions.DependencyModel.dll => 226
	i64 u0x16bf2a22df043a09, ; 109: System.IO.Pipes.dll => 56
	i64 u0x16ea2b318ad2d830, ; 110: System.Security.Cryptography.Algorithms => 122
	i64 u0x16eeae54c7ebcc08, ; 111: System.Reflection.dll => 100
	i64 u0x17125c9a85b4929f, ; 112: lib_netstandard.dll.so => 171
	i64 u0x1716866f7416792e, ; 113: lib_System.Security.AccessControl.dll.so => 120
	i64 u0x174f71c46216e44a, ; 114: Xamarin.KotlinX.Coroutines.Core => 357
	i64 u0x1752c12f1e1fc00c, ; 115: System.Core => 21
	i64 u0x1791d47293d97a1b, ; 116: lib_Npgsql.EntityFrameworkCore.PostgreSQL.dll.so => 254
	i64 u0x17b56e25558a5d36, ; 117: lib-hu-Microsoft.Maui.Controls.resources.dll.so => 374
	i64 u0x17f9358913beb16a, ; 118: System.Text.Encodings.Web => 139
	i64 u0x17fc580bfd8cdf43, ; 119: lib_Modules.Common.Domain.dll.so => 397
	i64 u0x1809fb23f29ba44a, ; 120: lib_System.Reflection.TypeExtensions.dll.so => 99
	i64 u0x18402a709e357f3b, ; 121: lib_Xamarin.KotlinX.Serialization.Core.Jvm.dll.so => 360
	i64 u0x18a9befae51bb361, ; 122: System.Net.WebClient => 79
	i64 u0x18f0ce884e87d89a, ; 123: nb/Microsoft.Maui.Controls.resources.dll => 380
	i64 u0x18facb3695ca9224, ; 124: Refit.HttpClientFactory => 257
	i64 u0x191e65bd9d4de607, ; 125: Modules.Common.Grpc.Contacts => 398
	i64 u0x19777fba3c41b398, ; 126: Xamarin.AndroidX.Startup.StartupRuntime.dll => 335
	i64 u0x19a4c090f14ebb66, ; 127: System.Security.Claims => 121
	i64 u0x1a63352be1054efd, ; 128: Microsoft.AspNetCore.Hosting.Server.Abstractions.dll => 200
	i64 u0x1a91866a319e9259, ; 129: lib_System.Collections.Concurrent.dll.so => 8
	i64 u0x1aac34d1917ba5d3, ; 130: lib_System.dll.so => 168
	i64 u0x1aad60783ffa3e5b, ; 131: lib-th-Microsoft.Maui.Controls.resources.dll.so => 389
	i64 u0x1aea8f1c3b282172, ; 132: lib_System.Net.Ping.dll.so => 71
	i64 u0x1b4b7a1d0d265fa2, ; 133: Xamarin.Android.Glide.DiskLruCache => 271
	i64 u0x1bbdb16cfa73e785, ; 134: Xamarin.AndroidX.Lifecycle.Runtime.Ktx.Android => 314
	i64 u0x1bc766e07b2b4241, ; 135: Xamarin.AndroidX.ResourceInspection.Annotation.dll => 329
	i64 u0x1bea5a36aa1ed8de, ; 136: Microsoft.AspNetCore.Http.Extensions => 203
	i64 u0x1c292b1598348d77, ; 137: Microsoft.Extensions.Diagnostics.dll => 227
	i64 u0x1c753b5ff15bce1b, ; 138: Mono.Android.Runtime.dll => 174
	i64 u0x1cd47467799d8250, ; 139: System.Threading.Tasks.dll => 148
	i64 u0x1d23eafdc6dc346c, ; 140: System.Globalization.Calendars.dll => 40
	i64 u0x1da4110562816681, ; 141: Xamarin.AndroidX.Security.SecurityCrypto.dll => 333
	i64 u0x1db6820994506bf5, ; 142: System.IO.FileSystem.AccessControl.dll => 47
	i64 u0x1dba6509cc55b56f, ; 143: lib_Google.Protobuf.dll.so => 184
	i64 u0x1dbb0c2c6a999acb, ; 144: System.Diagnostics.StackTrace => 30
	i64 u0x1e3d87657e9659bc, ; 145: Xamarin.AndroidX.Navigation.UI => 326
	i64 u0x1e71143913d56c10, ; 146: lib-ko-Microsoft.Maui.Controls.resources.dll.so => 378
	i64 u0x1e7c31185e2fb266, ; 147: lib_System.Threading.Tasks.Parallel.dll.so => 147
	i64 u0x1ed8fcce5e9b50a0, ; 148: Microsoft.Extensions.Options.dll => 238
	i64 u0x1f055d15d807e1b2, ; 149: System.Xml.XmlSerializer => 166
	i64 u0x1f198ea93d5594b5, ; 150: Microsoft.Extensions.Identity.Core => 232
	i64 u0x1f1ed22c1085f044, ; 151: lib_System.Diagnostics.FileVersionInfo.dll.so => 28
	i64 u0x1f61df9c5b94d2c1, ; 152: lib_System.Numerics.dll.so => 86
	i64 u0x1f750bb5421397de, ; 153: lib_Xamarin.AndroidX.Tracing.Tracing.dll.so => 337
	i64 u0x1fd24a4951087d1f, ; 154: Microsoft.AspNetCore.Mvc.Core.dll => 208
	i64 u0x20237ea48006d7a8, ; 155: lib_System.Net.WebClient.dll.so => 79
	i64 u0x209375905fcc1bad, ; 156: lib_System.IO.Compression.Brotli.dll.so => 43
	i64 u0x20fab3cf2dfbc8df, ; 157: lib_System.Diagnostics.Process.dll.so => 29
	i64 u0x2110167c128cba15, ; 158: System.Globalization => 42
	i64 u0x21419508838f7547, ; 159: System.Runtime.CompilerServices.VisualC => 105
	i64 u0x2174319c0d835bc9, ; 160: System.Runtime => 119
	i64 u0x2198e5bc8b7153fa, ; 161: Xamarin.AndroidX.Annotation.Experimental.dll => 276
	i64 u0x219ea1b751a4dee4, ; 162: lib_System.IO.Compression.ZipFile.dll.so => 45
	i64 u0x21cc7e445dcd5469, ; 163: System.Reflection.Emit.ILGeneration => 93
	i64 u0x220fd4f2e7c48170, ; 164: th/Microsoft.Maui.Controls.resources => 389
	i64 u0x224538d85ed15a82, ; 165: System.IO.Pipes => 56
	i64 u0x22908438c6bed1af, ; 166: lib_System.Threading.Timer.dll.so => 151
	i64 u0x237be844f1f812c7, ; 167: System.Threading.Thread.dll => 149
	i64 u0x23807c59646ec4f3, ; 168: lib_Microsoft.EntityFrameworkCore.dll.so => 215
	i64 u0x23852b3bdc9f7096, ; 169: System.Resources.ResourceManager => 102
	i64 u0x23986dd7e5d4fc01, ; 170: System.IO.FileSystem.Primitives.dll => 49
	i64 u0x2407aef2bbe8fadf, ; 171: System.Console => 20
	i64 u0x240abe014b27e7d3, ; 172: Xamarin.AndroidX.Core.dll => 291
	i64 u0x247619fe4413f8bf, ; 173: System.Runtime.Serialization.Primitives.dll => 116
	i64 u0x24de8d301281575e, ; 174: Xamarin.Android.Glide => 269
	i64 u0x252073cc3caa62c2, ; 175: fr/Microsoft.Maui.Controls.resources.dll => 370
	i64 u0x256b8d41255f01b1, ; 176: Xamarin.Google.Crypto.Tink.Android => 350
	i64 u0x25a0a7eff76ea08e, ; 177: SQLitePCLRaw.batteries_v2.dll => 259
	i64 u0x2662c629b96b0b30, ; 178: lib_Xamarin.Kotlin.StdLib.dll.so => 355
	i64 u0x268c1439f13bcc29, ; 179: lib_Microsoft.Extensions.Primitives.dll.so => 240
	i64 u0x26a670e154a9c54b, ; 180: System.Reflection.Extensions.dll => 96
	i64 u0x26d077d9678fe34f, ; 181: System.IO.dll => 58
	i64 u0x270a44600c921861, ; 182: System.IdentityModel.Tokens.Jwt => 266
	i64 u0x273f3515de5faf0d, ; 183: id/Microsoft.Maui.Controls.resources.dll => 375
	i64 u0x2742545f9094896d, ; 184: hr/Microsoft.Maui.Controls.resources => 373
	i64 u0x274d85d83ad40513, ; 185: lib_Xamarin.AndroidX.Window.WindowCore.dll.so => 346
	i64 u0x2759af78ab94d39b, ; 186: System.Net.WebSockets => 83
	i64 u0x27b2b16f3e9de038, ; 187: Xamarin.Google.Crypto.Tink.Android.dll => 350
	i64 u0x27b410442fad6cf1, ; 188: Java.Interop.dll => 172
	i64 u0x27b97e0d52c3034a, ; 189: System.Diagnostics.Debug => 26
	i64 u0x2801845a2c71fbfb, ; 190: System.Net.Primitives.dll => 72
	i64 u0x286835e259162700, ; 191: lib_Xamarin.AndroidX.ProfileInstaller.ProfileInstaller.dll.so => 327
	i64 u0x288f0dc6b8b36b5f, ; 192: Refit.dll => 256
	i64 u0x28b311fffbc0f8df, ; 193: Microsoft.AspNetCore.WebUtilities => 212
	i64 u0x28e52865585a1ebe, ; 194: Microsoft.Extensions.Diagnostics.Abstractions.dll => 228
	i64 u0x2949f3617a02c6b2, ; 195: Xamarin.AndroidX.ExifInterface => 301
	i64 u0x29aeab763a527e52, ; 196: lib_Xamarin.AndroidX.Navigation.Common.Android.dll.so => 322
	i64 u0x2a128783efe70ba0, ; 197: uk/Microsoft.Maui.Controls.resources.dll => 391
	i64 u0x2a3b095612184159, ; 198: lib_System.Net.NetworkInformation.dll.so => 70
	i64 u0x2a6507a5ffabdf28, ; 199: System.Diagnostics.TraceSource.dll => 33
	i64 u0x2ad156c8e1354139, ; 200: fi/Microsoft.Maui.Controls.resources => 369
	i64 u0x2ad5d6b13b7a3e04, ; 201: System.ComponentModel.DataAnnotations.dll => 14
	i64 u0x2af298f63581d886, ; 202: System.Text.RegularExpressions.dll => 141
	i64 u0x2af615542f04da50, ; 203: System.IdentityModel.Tokens.Jwt.dll => 266
	i64 u0x2afc1c4f898552ee, ; 204: lib_System.Formats.Asn1.dll.so => 38
	i64 u0x2b148910ed40fbf9, ; 205: zh-Hant/Microsoft.Maui.Controls.resources.dll => 395
	i64 u0x2b6989d78cba9a15, ; 206: Xamarin.AndroidX.Concurrent.Futures.dll => 287
	i64 u0x2c40db0dbedda89b, ; 207: lib_Microsoft.AspNetCore.WebUtilities.dll.so => 212
	i64 u0x2c8bd14bb93a7d82, ; 208: lib-pl-Microsoft.Maui.Controls.resources.dll.so => 382
	i64 u0x2c9f5b50547a5125, ; 209: Modules.Users.Infrastructure.dll => 402
	i64 u0x2cbd9262ca785540, ; 210: lib_System.Text.Encoding.CodePages.dll.so => 136
	i64 u0x2cc9e1fed6257257, ; 211: lib_System.Reflection.Emit.Lightweight.dll.so => 94
	i64 u0x2cd723e9fe623c7c, ; 212: lib_System.Private.Xml.Linq.dll.so => 90
	i64 u0x2d169d318a968379, ; 213: System.Threading.dll => 152
	i64 u0x2d20145f27cfc1d2, ; 214: Xamarin.AndroidX.Window.WindowCore.Jvm.dll => 347
	i64 u0x2d47774b7d993f59, ; 215: sv/Microsoft.Maui.Controls.resources.dll => 388
	i64 u0x2d5ffcae1ad0aaca, ; 216: System.Data.dll => 24
	i64 u0x2db915caf23548d2, ; 217: System.Text.Json.dll => 140
	i64 u0x2dcaa0bb15a4117a, ; 218: System.IO.UnmanagedMemoryStream.dll => 57
	i64 u0x2e2ced2c3c6a1edc, ; 219: lib_System.Threading.AccessControl.dll.so => 142
	i64 u0x2e54220bd5db87a1, ; 220: Moduels.Workouts.DTO.dll => 396
	i64 u0x2e5a40c319acb800, ; 221: System.IO.FileSystem => 51
	i64 u0x2e6f1f226821322a, ; 222: el/Microsoft.Maui.Controls.resources.dll => 367
	i64 u0x2ed4e41fc62539c3, ; 223: StackExchange.Redis => 263
	i64 u0x2f02f94df3200fe5, ; 224: System.Diagnostics.Process => 29
	i64 u0x2f2e98e1c89b1aff, ; 225: System.Xml.ReaderWriter => 160
	i64 u0x2f5911d9ba814e4e, ; 226: System.Diagnostics.Tracing => 34
	i64 u0x2f84070a459bc31f, ; 227: lib_System.Xml.dll.so => 167
	i64 u0x2feb4d2fcda05cfd, ; 228: Microsoft.Extensions.Caching.Abstractions.dll => 218
	i64 u0x2ff49de6a71764a1, ; 229: lib_Microsoft.Extensions.Http.dll.so => 231
	i64 u0x3033937e1dfee52b, ; 230: FluentValidation => 183
	i64 u0x309ee9eeec09a71e, ; 231: lib_Xamarin.AndroidX.Fragment.dll.so => 302
	i64 u0x309f2bedefa9a318, ; 232: Microsoft.IdentityModel.Abstractions => 242
	i64 u0x30c6dda129408828, ; 233: System.IO.IsolatedStorage => 52
	i64 u0x30ea94feb21cbb08, ; 234: AKSoftware.Localization.MultiLanguages.dll => 178
	i64 u0x31195fef5d8fb552, ; 235: _Microsoft.Android.Resource.Designer.dll => 403
	i64 u0x312c8ed623cbfc8d, ; 236: Xamarin.AndroidX.Window.dll => 345
	i64 u0x31496b779ed0663d, ; 237: lib_System.Reflection.DispatchProxy.dll.so => 92
	i64 u0x32243413e774362a, ; 238: Xamarin.AndroidX.CardView.dll => 283
	i64 u0x3235427f8d12dae1, ; 239: lib_System.Drawing.Primitives.dll.so => 35
	i64 u0x329753a17a517811, ; 240: fr/Microsoft.Maui.Controls.resources => 370
	i64 u0x32aa989ff07a84ff, ; 241: lib_System.Xml.ReaderWriter.dll.so => 160
	i64 u0x33829542f112d59b, ; 242: System.Collections.Immutable => 9
	i64 u0x33a31443733849fe, ; 243: lib-es-Microsoft.Maui.Controls.resources.dll.so => 368
	i64 u0x341abc357fbb4ebf, ; 244: lib_System.Net.Sockets.dll.so => 78
	i64 u0x346a212343615ac5, ; 245: lib_System.Linq.AsyncEnumerable.dll.so => 59
	i64 u0x3496c1e2dcaf5ecc, ; 246: lib_System.IO.Pipes.AccessControl.dll.so => 55
	i64 u0x34dfd74fe2afcf37, ; 247: Microsoft.Maui => 248
	i64 u0x34e292762d9615df, ; 248: cs/Microsoft.Maui.Controls.resources.dll => 364
	i64 u0x3508234247f48404, ; 249: Microsoft.Maui.Controls => 246
	i64 u0x353590da528c9d22, ; 250: System.ComponentModel.Annotations => 13
	i64 u0x3549870798b4cd30, ; 251: lib_Xamarin.AndroidX.ViewPager2.dll.so => 344
	i64 u0x355282fc1c909694, ; 252: Microsoft.Extensions.Configuration => 221
	i64 u0x3552fc5d578f0fbf, ; 253: Xamarin.AndroidX.Arch.Core.Common => 280
	i64 u0x355c649948d55d97, ; 254: lib_System.Runtime.Intrinsics.dll.so => 111
	i64 u0x3598b7b6237a86b6, ; 255: lib_Microsoft.AspNetCore.Authentication.dll.so => 189
	i64 u0x35ea9d1c6834bc8c, ; 256: Xamarin.AndroidX.Lifecycle.ViewModel.Ktx.dll => 317
	i64 u0x36263608556d5d42, ; 257: Npgsql.dll => 253
	i64 u0x3628ab68db23a01a, ; 258: lib_System.Diagnostics.Tools.dll.so => 32
	i64 u0x3673b042508f5b6b, ; 259: lib_System.Runtime.Extensions.dll.so => 106
	i64 u0x36740f1a8ecdc6c4, ; 260: System.Numerics => 86
	i64 u0x36b2b50fdf589ae2, ; 261: System.Reflection.Emit.Lightweight => 94
	i64 u0x36cada77dc79928b, ; 262: System.IO.MemoryMappedFiles => 53
	i64 u0x374ef46b06791af6, ; 263: System.Reflection.Primitives.dll => 98
	i64 u0x375a0c086b00470b, ; 264: Microsoft.AspNetCore.Authentication.dll => 189
	i64 u0x376bf93e521a5417, ; 265: lib_Xamarin.Jetbrains.Annotations.dll.so => 353
	i64 u0x37bc29f3183003b6, ; 266: lib_System.IO.dll.so => 58
	i64 u0x37fd73cba07e0b9d, ; 267: lib_Microsoft.AspNetCore.Cryptography.Internal.dll.so => 195
	i64 u0x380134e03b1e160a, ; 268: System.Collections.Immutable.dll => 9
	i64 u0x38049b5c59b39324, ; 269: System.Runtime.CompilerServices.Unsafe => 104
	i64 u0x385c17636bb6fe6e, ; 270: Xamarin.AndroidX.CustomView.dll => 295
	i64 u0x38869c811d74050e, ; 271: System.Net.NameResolution.dll => 69
	i64 u0x38f71e7a64343c93, ; 272: lib_Microsoft.AspNetCore.Authorization.Policy.dll.so => 194
	i64 u0x393c226616977fdb, ; 273: lib_Xamarin.AndroidX.ViewPager.dll.so => 343
	i64 u0x395e37c3334cf82a, ; 274: lib-ca-Microsoft.Maui.Controls.resources.dll.so => 363
	i64 u0x39aa39fda111d9d3, ; 275: Newtonsoft.Json => 252
	i64 u0x39c3107c28752af1, ; 276: lib_Microsoft.Extensions.FileProviders.Abstractions.dll.so => 229
	i64 u0x3ab5859054645f72, ; 277: System.Security.Cryptography.Primitives.dll => 127
	i64 u0x3ad75090c3fac0e9, ; 278: lib_Xamarin.AndroidX.ResourceInspection.Annotation.dll.so => 329
	i64 u0x3ae44ac43a1fbdbb, ; 279: System.Runtime.Serialization => 118
	i64 u0x3b860f9932505633, ; 280: lib_System.Text.Encoding.Extensions.dll.so => 137
	i64 u0x3be99b43dd39dd37, ; 281: Xamarin.AndroidX.SavedState.SavedState.Android => 331
	i64 u0x3bea9ebe8c027c01, ; 282: lib_Microsoft.IdentityModel.Tokens.dll.so => 245
	i64 u0x3c3aafb6b3a00bf6, ; 283: lib_System.Security.Cryptography.X509Certificates.dll.so => 128
	i64 u0x3c4049146b59aa90, ; 284: System.Runtime.InteropServices.JavaScript => 108
	i64 u0x3c7c495f58ac5ee9, ; 285: Xamarin.Kotlin.StdLib => 355
	i64 u0x3c7e5ed3d5db71bb, ; 286: System.Security => 133
	i64 u0x3ca05b43ec08224f, ; 287: Microsoft.AspNetCore.Http.Extensions.dll => 203
	i64 u0x3cd9d281d402eb9b, ; 288: Xamarin.AndroidX.Browser.dll => 282
	i64 u0x3d1c50cc001a991e, ; 289: Xamarin.Google.Guava.ListenableFuture.dll => 352
	i64 u0x3d2b1913edfc08d7, ; 290: lib_System.Threading.ThreadPool.dll.so => 150
	i64 u0x3d46f0b995082740, ; 291: System.Xml.Linq => 159
	i64 u0x3d8a8f400514a790, ; 292: Xamarin.AndroidX.Fragment.Ktx.dll => 303
	i64 u0x3d9c2a242b040a50, ; 293: lib_Xamarin.AndroidX.Core.dll.so => 291
	i64 u0x3da7781d6333a8fe, ; 294: SQLitePCLRaw.batteries_v2 => 259
	i64 u0x3daa14724d8f58e8, ; 295: Google.Protobuf.dll => 184
	i64 u0x3dbb6b9f5ab90fa7, ; 296: lib_Xamarin.AndroidX.DynamicAnimation.dll.so => 298
	i64 u0x3e5441657549b213, ; 297: Xamarin.AndroidX.ResourceInspection.Annotation => 329
	i64 u0x3e57d4d195c53c2e, ; 298: System.Reflection.TypeExtensions => 99
	i64 u0x3e580c35ecfc1247, ; 299: lib_Microsoft.AspNetCore.Http.dll.so => 201
	i64 u0x3e616ab4ed1f3f15, ; 300: lib_System.Data.dll.so => 24
	i64 u0x3f1d226e6e06db7e, ; 301: Xamarin.AndroidX.SlidingPaneLayout.dll => 334
	i64 u0x3f510adf788828dd, ; 302: System.Threading.Tasks.Extensions => 146
	i64 u0x3f6f5914291cdcf7, ; 303: Microsoft.Extensions.Hosting.Abstractions => 230
	i64 u0x407a10bb4bf95829, ; 304: lib_Xamarin.AndroidX.Navigation.Common.dll.so => 321
	i64 u0x40c98b6bd77346d4, ; 305: Microsoft.VisualBasic.dll => 3
	i64 u0x41640f0d7a3d1d80, ; 306: lib_Microsoft.Extensions.Caching.StackExchangeRedis.dll.so => 220
	i64 u0x41833cf766d27d96, ; 307: mscorlib => 170
	i64 u0x41cab042be111c34, ; 308: lib_Xamarin.AndroidX.AppCompat.AppCompatResources.dll.so => 279
	i64 u0x423a9ecc4d905a88, ; 309: lib_System.Resources.ResourceManager.dll.so => 102
	i64 u0x423bf51ae7def810, ; 310: System.Xml.XPath => 164
	i64 u0x42462ff15ddba223, ; 311: System.Resources.Reader.dll => 101
	i64 u0x4291015ff4e5ef71, ; 312: Xamarin.AndroidX.Core.ViewTree.dll => 293
	i64 u0x4294a05ba79b4e3b, ; 313: Microsoft.AspNetCore.Authentication.Cookies.dll => 191
	i64 u0x42a31b86e6ccc3f0, ; 314: System.Diagnostics.Contracts => 25
	i64 u0x42d76b1d438bed3f, ; 315: Microsoft.AspNetCore.Identity => 205
	i64 u0x430e95b891249788, ; 316: lib_System.Reflection.Emit.dll.so => 95
	i64 u0x43375950ec7c1b6a, ; 317: netstandard.dll => 171
	i64 u0x434c4e1d9284cdae, ; 318: Mono.Android.dll => 175
	i64 u0x43505013578652a0, ; 319: lib_Xamarin.AndroidX.Activity.Ktx.dll.so => 274
	i64 u0x437d06c381ed575a, ; 320: lib_Microsoft.VisualBasic.dll.so => 3
	i64 u0x43950f84de7cc79a, ; 321: pl/Microsoft.Maui.Controls.resources.dll => 382
	i64 u0x43c077442b230f64, ; 322: Xamarin.AndroidX.Tracing.Tracing.Android => 338
	i64 u0x43e8ca5bc927ff37, ; 323: lib_Xamarin.AndroidX.Emoji2.ViewsHelper.dll.so => 300
	i64 u0x448bd33429269b19, ; 324: Microsoft.CSharp => 1
	i64 u0x4499fa3c8e494654, ; 325: lib_System.Runtime.Serialization.Primitives.dll.so => 116
	i64 u0x4515080865a951a5, ; 326: Xamarin.Kotlin.StdLib.dll => 355
	i64 u0x45344658e8f1a46d, ; 327: Microsoft.AspNetCore.Authentication.Core => 192
	i64 u0x453c1277f85cf368, ; 328: lib_Microsoft.EntityFrameworkCore.Abstractions.dll.so => 216
	i64 u0x4545802489b736b9, ; 329: Xamarin.AndroidX.Fragment.Ktx => 303
	i64 u0x454b4d1e66bb783c, ; 330: Xamarin.AndroidX.Lifecycle.Process => 310
	i64 u0x458d2df79ac57c1d, ; 331: lib_System.IdentityModel.Tokens.Jwt.dll.so => 266
	i64 u0x45c40276a42e283e, ; 332: System.Diagnostics.TraceSource => 33
	i64 u0x45d443f2a29adc37, ; 333: System.AppContext.dll => 6
	i64 u0x45fcc9fd66f25095, ; 334: Microsoft.Extensions.DependencyModel => 226
	i64 u0x463d680a1dec0810, ; 335: System.Security.Cryptography.Xml.dll => 268
	i64 u0x46a4213bc97fe5ae, ; 336: lib-ru-Microsoft.Maui.Controls.resources.dll.so => 386
	i64 u0x47358bd471172e1d, ; 337: lib_System.Xml.Linq.dll.so => 159
	i64 u0x4747e19ad6a1d4bb, ; 338: Grpc.Net.Common => 188
	i64 u0x47daf4e1afbada10, ; 339: pt/Microsoft.Maui.Controls.resources => 384
	i64 u0x480c0a47dd42dd81, ; 340: lib_System.IO.MemoryMappedFiles.dll.so => 53
	i64 u0x497eb1d03ac05c8a, ; 341: lib_Microsoft.Extensions.WebEncoders.dll.so => 241
	i64 u0x49e952f19a4e2022, ; 342: System.ObjectModel => 87
	i64 u0x49ea01c721d701b5, ; 343: lib_Microsoft.Net.Http.Headers.dll.so => 251
	i64 u0x49f9e6948a8131e4, ; 344: lib_Xamarin.AndroidX.VersionedParcelable.dll.so => 342
	i64 u0x4a5667b2462a664b, ; 345: lib_Xamarin.AndroidX.Navigation.UI.dll.so => 326
	i64 u0x4a7a18981dbd56bc, ; 346: System.IO.Compression.FileSystem.dll => 44
	i64 u0x4aa5c60350917c06, ; 347: lib_Xamarin.AndroidX.Lifecycle.LiveData.Core.Ktx.dll.so => 309
	i64 u0x4b07a0ed0ab33ff4, ; 348: System.Runtime.Extensions.dll => 106
	i64 u0x4b576d47ac054f3c, ; 349: System.IO.FileSystem.AccessControl => 47
	i64 u0x4b5cc074fafbe58e, ; 350: Microsoft.AspNetCore.ResponseCaching.Abstractions => 209
	i64 u0x4b7b6532ded934b7, ; 351: System.Text.Json => 140
	i64 u0x4bbc7df476e2e556, ; 352: Microsoft.AspNetCore.Mvc.Abstractions.dll => 207
	i64 u0x4c2029a97af23a8d, ; 353: Xamarin.AndroidX.Lifecycle.ViewModelSavedState.Android => 319
	i64 u0x4c7755cf07ad2d5f, ; 354: System.Net.Http.Json.dll => 65
	i64 u0x4ca014ceac582c86, ; 355: Microsoft.EntityFrameworkCore.Relational.dll => 217
	i64 u0x4cc5f15266470798, ; 356: lib_Xamarin.AndroidX.Loader.dll.so => 320
	i64 u0x4cf6f67dc77aacd2, ; 357: System.Net.NetworkInformation.dll => 70
	i64 u0x4d3183dd245425d4, ; 358: System.Net.WebSockets.Client.dll => 82
	i64 u0x4d479f968a05e504, ; 359: System.Linq.Expressions.dll => 60
	i64 u0x4d55a010ffc4faff, ; 360: System.Private.Xml => 91
	i64 u0x4d5cbe77561c5b2e, ; 361: System.Web.dll => 157
	i64 u0x4d77512dbd86ee4c, ; 362: lib_Xamarin.AndroidX.Arch.Core.Common.dll.so => 280
	i64 u0x4d7793536e79c309, ; 363: System.ServiceProcess => 135
	i64 u0x4d95fccc1f67c7ca, ; 364: System.Runtime.Loader.dll => 112
	i64 u0x4db014bf0ff1c9c1, ; 365: System.Linq.AsyncEnumerable => 59
	i64 u0x4dcf44c3c9b076a2, ; 366: it/Microsoft.Maui.Controls.resources.dll => 376
	i64 u0x4dd9247f1d2c3235, ; 367: Xamarin.AndroidX.Loader.dll => 320
	i64 u0x4e2aeee78e2c4a87, ; 368: Xamarin.AndroidX.ProfileInstaller.ProfileInstaller => 327
	i64 u0x4e32f00cb0937401, ; 369: Mono.Android.Runtime => 174
	i64 u0x4e3369190c3dcd08, ; 370: Microsoft.Extensions.Identity.Stores => 233
	i64 u0x4e5eea4668ac2b18, ; 371: System.Text.Encoding.CodePages => 136
	i64 u0x4ebd0c4b82c5eefc, ; 372: lib_System.Threading.Channels.dll.so => 143
	i64 u0x4ee8eaa9c9c1151a, ; 373: System.Globalization.Calendars => 40
	i64 u0x4f21ee6ef9eb527e, ; 374: ca/Microsoft.Maui.Controls.resources => 363
	i64 u0x4fd5f3ee53d0a4f0, ; 375: SQLitePCLRaw.lib.e_sqlite3.android => 261
	i64 u0x4fdc964ec1888e25, ; 376: lib_Microsoft.Extensions.Configuration.Binder.dll.so => 223
	i64 u0x4ff55724131c908c, ; 377: lib_Microsoft.AspNetCore.Mvc.Core.dll.so => 208
	i64 u0x4ffd65baff757598, ; 378: Microsoft.IdentityModel.Tokens => 245
	i64 u0x5037f0be3c28c7a3, ; 379: lib_Microsoft.Maui.Controls.dll.so => 246
	i64 u0x508c1fa6b57728d9, ; 380: Grpc.Net.Common.dll => 188
	i64 u0x50c3a29b21050d45, ; 381: System.Linq.Parallel.dll => 61
	i64 u0x5112ed116d87baf8, ; 382: CommunityToolkit.Mvvm => 181
	i64 u0x5116b21580ae6eb0, ; 383: Microsoft.Extensions.Configuration.Binder.dll => 223
	i64 u0x5131bbe80989093f, ; 384: Xamarin.AndroidX.Lifecycle.ViewModel.Android.dll => 316
	i64 u0x516324a5050a7e3c, ; 385: System.Net.WebProxy => 81
	i64 u0x516d6f0b21a303de, ; 386: lib_System.Diagnostics.Contracts.dll.so => 25
	i64 u0x51bb8a2afe774e32, ; 387: System.Drawing => 36
	i64 u0x5216f09c5c4c95c8, ; 388: Microsoft.AspNetCore.Authentication.Abstractions => 190
	i64 u0x5247c5c32a4140f0, ; 389: System.Resources.Reader => 101
	i64 u0x526bb15e3c386364, ; 390: Xamarin.AndroidX.Lifecycle.Runtime.Ktx.dll => 313
	i64 u0x526ce79eb8e90527, ; 391: lib_System.Net.Primitives.dll.so => 72
	i64 u0x527497f521875686, ; 392: Microsoft.AspNetCore.Http.Abstractions => 202
	i64 u0x5277169428c6ebf6, ; 393: lib_Grpc.Net.Common.dll.so => 188
	i64 u0x52829f00b4467c38, ; 394: lib_System.Data.Common.dll.so => 22
	i64 u0x529ffe06f39ab8db, ; 395: Xamarin.AndroidX.Core => 291
	i64 u0x52ff996554dbf352, ; 396: Microsoft.Maui.Graphics => 250
	i64 u0x5324b9a9dedb24aa, ; 397: Microsoft.AspNetCore.Cryptography.Internal => 195
	i64 u0x535f7e40e8fef8af, ; 398: lib-sk-Microsoft.Maui.Controls.resources.dll.so => 387
	i64 u0x53978aac584c666e, ; 399: lib_System.Security.Cryptography.Cng.dll.so => 123
	i64 u0x53a96d5c86c9e194, ; 400: System.Net.NetworkInformation => 70
	i64 u0x53be1038a61e8d44, ; 401: System.Runtime.InteropServices.RuntimeInformation.dll => 109
	i64 u0x53c3014b9437e684, ; 402: lib-zh-HK-Microsoft.Maui.Controls.resources.dll.so => 393
	i64 u0x53d666fa678b6cea, ; 403: Microsoft.DotNet.PlatformAbstractions => 214
	i64 u0x5435e6f049e9bc37, ; 404: System.Security.Claims.dll => 121
	i64 u0x54795225dd1587af, ; 405: lib_System.Runtime.dll.so => 119
	i64 u0x547a34f14e5f6210, ; 406: Xamarin.AndroidX.Lifecycle.Common.dll => 305
	i64 u0x54a0124adceadbc7, ; 407: Microsoft.AspNetCore.DataProtection => 197
	i64 u0x54b851bc9b470503, ; 408: Xamarin.AndroidX.Navigation.Common.Android => 322
	i64 u0x556e8b63b660ab8b, ; 409: Xamarin.AndroidX.Lifecycle.Common.Jvm.dll => 306
	i64 u0x5588627c9a108ec9, ; 410: System.Collections.Specialized => 11
	i64 u0x55a898e4f42e3fae, ; 411: Microsoft.VisualBasic.Core.dll => 2
	i64 u0x55fa0c610fe93bb1, ; 412: lib_System.Security.Cryptography.OpenSsl.dll.so => 126
	i64 u0x56442b99bc64bb47, ; 413: System.Runtime.Serialization.Xml.dll => 117
	i64 u0x56a8b26e1aeae27b, ; 414: System.Threading.Tasks.Dataflow => 145
	i64 u0x56f932d61e93c07f, ; 415: System.Globalization.Extensions => 41
	i64 u0x571c5cfbec5ae8e2, ; 416: System.Private.Uri => 89
	i64 u0x576499c9f52fea31, ; 417: Xamarin.AndroidX.Annotation => 275
	i64 u0x578cd35c91d7b347, ; 418: lib_SQLitePCLRaw.core.dll.so => 260
	i64 u0x579a06fed6eec900, ; 419: System.Private.CoreLib.dll => 177
	i64 u0x57adda3c951abb33, ; 420: Microsoft.Extensions.Hosting.Abstractions.dll => 230
	i64 u0x57c542c14049b66d, ; 421: System.Diagnostics.DiagnosticSource => 27
	i64 u0x581a8bd5cfda563e, ; 422: System.Threading.Timer => 151
	i64 u0x584ac38e21d2fde1, ; 423: Microsoft.Extensions.Configuration.Binder => 223
	i64 u0x58601b2dda4a27b9, ; 424: lib-ja-Microsoft.Maui.Controls.resources.dll.so => 377
	i64 u0x58688d9af496b168, ; 425: Microsoft.Extensions.DependencyInjection.dll => 224
	i64 u0x587f59a16b329d9c, ; 426: Microsoft.Net.Http.Headers => 251
	i64 u0x588c167a79db6bfb, ; 427: lib_Xamarin.Google.ErrorProne.Annotations.dll.so => 351
	i64 u0x5906028ae5151104, ; 428: Xamarin.AndroidX.Activity.Ktx => 274
	i64 u0x595a356d23e8da9a, ; 429: lib_Microsoft.CSharp.dll.so => 1
	i64 u0x59c270386bf40142, ; 430: Microsoft.AspNetCore.Hosting.Server.Abstractions => 200
	i64 u0x59f9e60b9475085f, ; 431: lib_Xamarin.AndroidX.Annotation.Experimental.dll.so => 276
	i64 u0x5a745f5101a75527, ; 432: lib_System.IO.Compression.FileSystem.dll.so => 44
	i64 u0x5a89a886ae30258d, ; 433: lib_Xamarin.AndroidX.CoordinatorLayout.dll.so => 290
	i64 u0x5a8f6699f4a1caa9, ; 434: lib_System.Threading.dll.so => 152
	i64 u0x5ae9cd33b15841bf, ; 435: System.ComponentModel => 18
	i64 u0x5b41ce8de0a5118c, ; 436: WorkoutLogg => 0
	i64 u0x5b54391bdc6fcfe6, ; 437: System.Private.DataContractSerialization => 88
	i64 u0x5b5f0e240a06a2a2, ; 438: da/Microsoft.Maui.Controls.resources.dll => 365
	i64 u0x5b8109e8e14c5e3e, ; 439: System.Globalization.Extensions.dll => 41
	i64 u0x5bddd04d72a9e350, ; 440: Xamarin.AndroidX.Lifecycle.LiveData.Core.Ktx => 309
	i64 u0x5bdf16b09da116ab, ; 441: Xamarin.AndroidX.Collection => 284
	i64 u0x5beca398fe6dce6b, ; 442: lib_Microsoft.AspNetCore.Mvc.Abstractions.dll.so => 207
	i64 u0x5c019d5266093159, ; 443: lib_Xamarin.AndroidX.Lifecycle.Runtime.Ktx.Android.dll.so => 314
	i64 u0x5c30a4a35f9cc8c4, ; 444: lib_System.Reflection.Extensions.dll.so => 96
	i64 u0x5c393624b8176517, ; 445: lib_Microsoft.Extensions.Logging.dll.so => 234
	i64 u0x5c53c29f5073b0c9, ; 446: System.Diagnostics.FileVersionInfo => 28
	i64 u0x5c5633a83721db9f, ; 447: WorkoutLogg.dll => 0
	i64 u0x5c87463c575c7616, ; 448: lib_System.Globalization.Extensions.dll.so => 41
	i64 u0x5cbe0283eb598ee8, ; 449: Microsoft.AspNetCore.Routing => 210
	i64 u0x5d0a4a29b02d9d3c, ; 450: System.Net.WebHeaderCollection.dll => 80
	i64 u0x5d40c9b15181641f, ; 451: lib_Xamarin.AndroidX.Emoji2.dll.so => 299
	i64 u0x5d6ca10d35e9485b, ; 452: lib_Xamarin.AndroidX.Concurrent.Futures.dll.so => 287
	i64 u0x5d7ec76c1c703055, ; 453: System.Threading.Tasks.Parallel => 147
	i64 u0x5db0cbbd1028510e, ; 454: lib_System.Runtime.InteropServices.dll.so => 110
	i64 u0x5db30905d3e5013b, ; 455: Xamarin.AndroidX.Collection.Jvm.dll => 285
	i64 u0x5e467bc8f09ad026, ; 456: System.Collections.Specialized.dll => 11
	i64 u0x5e5173b3208d97e7, ; 457: System.Runtime.Handles.dll => 107
	i64 u0x5ea92fdb19ec8c4c, ; 458: System.Text.Encodings.Web.dll => 139
	i64 u0x5eb8046dd40e9ac3, ; 459: System.ComponentModel.Primitives => 16
	i64 u0x5ec272d219c9aba4, ; 460: System.Security.Cryptography.Csp.dll => 124
	i64 u0x5eee1376d94c7f5e, ; 461: System.Net.HttpListener.dll => 67
	i64 u0x5f36ccf5c6a57e24, ; 462: System.Xml.ReaderWriter.dll => 160
	i64 u0x5f4294b9b63cb842, ; 463: System.Data.Common => 22
	i64 u0x5f7399e166075632, ; 464: lib_SQLitePCLRaw.lib.e_sqlite3.android.dll.so => 261
	i64 u0x5f9a2d823f664957, ; 465: lib-el-Microsoft.Maui.Controls.resources.dll.so => 367
	i64 u0x5fa6da9c3cd8142a, ; 466: lib_Xamarin.KotlinX.Serialization.Core.dll.so => 359
	i64 u0x5fac98e0b37a5b9d, ; 467: System.Runtime.CompilerServices.Unsafe.dll => 104
	i64 u0x5fd02402d97cdaab, ; 468: lib_Microsoft.Extensions.ObjectPool.dll.so => 237
	i64 u0x609f4b7b63d802d4, ; 469: lib_Microsoft.Extensions.DependencyInjection.dll.so => 224
	i64 u0x60cd4e33d7e60134, ; 470: Xamarin.KotlinX.Coroutines.Core.Jvm => 358
	i64 u0x60f62d786afcf130, ; 471: System.Memory => 64
	i64 u0x61bb78c89f867353, ; 472: System.IO => 58
	i64 u0x61be8d1299194243, ; 473: Microsoft.Maui.Controls.Xaml => 247
	i64 u0x61d2cba29557038f, ; 474: de/Microsoft.Maui.Controls.resources => 366
	i64 u0x61d88f399afb2f45, ; 475: lib_System.Runtime.Loader.dll.so => 112
	i64 u0x622eef6f9e59068d, ; 476: System.Private.CoreLib => 177
	i64 u0x639fb99a7bef11de, ; 477: Xamarin.AndroidX.Navigation.Runtime.Android.dll => 325
	i64 u0x63d5e3aa4ef9b931, ; 478: Xamarin.KotlinX.Coroutines.Android.dll => 356
	i64 u0x63f1f6883c1e23c2, ; 479: lib_System.Collections.Immutable.dll.so => 9
	i64 u0x6400f68068c1e9f1, ; 480: Xamarin.Google.Android.Material.dll => 348
	i64 u0x640e3b14dbd325c2, ; 481: System.Security.Cryptography.Algorithms.dll => 122
	i64 u0x64587004560099b9, ; 482: System.Reflection => 100
	i64 u0x64b1529a438a3c45, ; 483: lib_System.Runtime.Handles.dll.so => 107
	i64 u0x64b61dd9da8a4d57, ; 484: System.Net.ServerSentEvents.dll => 76
	i64 u0x655a00a848fc3334, ; 485: lib_Modules.Users.Infrastructure.dll.so => 402
	i64 u0x6565fba2cd8f235b, ; 486: Xamarin.AndroidX.Lifecycle.ViewModel.Ktx => 317
	i64 u0x658f524e4aba7dad, ; 487: CommunityToolkit.Maui.dll => 179
	i64 u0x659dc45417570048, ; 488: Refit => 256
	i64 u0x65ecac39144dd3cc, ; 489: Microsoft.Maui.Controls.dll => 246
	i64 u0x65ece51227bfa724, ; 490: lib_System.Runtime.Numerics.dll.so => 113
	i64 u0x661722438787b57f, ; 491: Xamarin.AndroidX.Annotation.Jvm.dll => 277
	i64 u0x6679b2337ee6b22a, ; 492: lib_System.IO.FileSystem.Primitives.dll.so => 49
	i64 u0x667c66a03dd97d40, ; 493: System.Linq.AsyncEnumerable.dll => 59
	i64 u0x6692e924eade1b29, ; 494: lib_System.Console.dll.so => 20
	i64 u0x66a4e5c6a3fb0bae, ; 495: lib_Xamarin.AndroidX.Lifecycle.ViewModel.Android.dll.so => 316
	i64 u0x66d13304ce1a3efa, ; 496: Xamarin.AndroidX.CursorAdapter => 294
	i64 u0x674303f65d8fad6f, ; 497: lib_System.Net.Quic.dll.so => 73
	i64 u0x6756ca4cad62e9d6, ; 498: lib_Xamarin.AndroidX.ConstraintLayout.Core.dll.so => 289
	i64 u0x67c0802770244408, ; 499: System.Windows.dll => 158
	i64 u0x68100b69286e27cd, ; 500: lib_System.Formats.Tar.dll.so => 39
	i64 u0x68558ec653afa616, ; 501: lib-da-Microsoft.Maui.Controls.resources.dll.so => 365
	i64 u0x6872ec7a2e36b1ac, ; 502: System.Drawing.Primitives.dll => 35
	i64 u0x68fbbbe2eb455198, ; 503: System.Formats.Asn1 => 38
	i64 u0x69063fc0ba8e6bdd, ; 504: he/Microsoft.Maui.Controls.resources.dll => 371
	i64 u0x699dffb2427a2d71, ; 505: SQLitePCLRaw.lib.e_sqlite3.android.dll => 261
	i64 u0x6a4d7577b2317255, ; 506: System.Runtime.InteropServices.dll => 110
	i64 u0x6ace3b74b15ee4a4, ; 507: nb/Microsoft.Maui.Controls.resources => 380
	i64 u0x6afcedb171067e2b, ; 508: System.Core.dll => 21
	i64 u0x6ba55548e7efe195, ; 509: Confluent.Kafka => 182
	i64 u0x6bddb6dc9c6f52a0, ; 510: YamlDotNet => 361
	i64 u0x6bef98e124147c24, ; 511: Xamarin.Jetbrains.Annotations => 353
	i64 u0x6ce874bff138ce2b, ; 512: Xamarin.AndroidX.Lifecycle.ViewModel.dll => 315
	i64 u0x6d12bfaa99c72b1f, ; 513: lib_Microsoft.Maui.Graphics.dll.so => 250
	i64 u0x6d70755158ca866e, ; 514: lib_System.ComponentModel.EventBasedAsync.dll.so => 15
	i64 u0x6d79993361e10ef2, ; 515: Microsoft.Extensions.Primitives => 240
	i64 u0x6d7eeca99577fc8b, ; 516: lib_System.Net.WebProxy.dll.so => 81
	i64 u0x6d8515b19946b6a2, ; 517: System.Net.WebProxy.dll => 81
	i64 u0x6d86d56b84c8eb71, ; 518: lib_Xamarin.AndroidX.CursorAdapter.dll.so => 294
	i64 u0x6d9bea6b3e895cf7, ; 519: Microsoft.Extensions.Primitives.dll => 240
	i64 u0x6e098fb160d68d4f, ; 520: Modules.Users.Domain => 400
	i64 u0x6e25a02c3833319a, ; 521: lib_Xamarin.AndroidX.Navigation.Fragment.dll.so => 323
	i64 u0x6e79c6bd8627412a, ; 522: Xamarin.AndroidX.SavedState.SavedState.Ktx => 332
	i64 u0x6e838d9a2a6f6c9e, ; 523: lib_System.ValueTuple.dll.so => 155
	i64 u0x6e9965ce1095e60a, ; 524: lib_System.Core.dll.so => 21
	i64 u0x6fd2265da78b93a4, ; 525: lib_Microsoft.Maui.dll.so => 248
	i64 u0x6fdfc7de82c33008, ; 526: cs/Microsoft.Maui.Controls.resources => 364
	i64 u0x6ffc4967cc47ba57, ; 527: System.IO.FileSystem.Watcher.dll => 50
	i64 u0x701cd46a1c25a5fe, ; 528: System.IO.FileSystem.dll => 51
	i64 u0x70e99f48c05cb921, ; 529: tr/Microsoft.Maui.Controls.resources.dll => 390
	i64 u0x70fd3deda22442d2, ; 530: lib-nb-Microsoft.Maui.Controls.resources.dll.so => 380
	i64 u0x71485e7ffdb4b958, ; 531: System.Reflection.Extensions => 96
	i64 u0x7162a2fce67a945f, ; 532: lib_Xamarin.Android.Glide.Annotations.dll.so => 270
	i64 u0x717530326f808838, ; 533: lib_Microsoft.Extensions.Diagnostics.Abstractions.dll.so => 228
	i64 u0x71a495ea3761dde8, ; 534: lib-it-Microsoft.Maui.Controls.resources.dll.so => 376
	i64 u0x71ad672adbe48f35, ; 535: System.ComponentModel.Primitives.dll => 16
	i64 u0x71bc142d620e986a, ; 536: lib_System.Security.Cryptography.Pkcs.dll.so => 267
	i64 u0x720f102581a4a5c8, ; 537: Xamarin.AndroidX.Core.ViewTree => 293
	i64 u0x725f5a9e82a45c81, ; 538: System.Security.Cryptography.Encoding => 125
	i64 u0x72b1fb4109e08d7b, ; 539: lib-hr-Microsoft.Maui.Controls.resources.dll.so => 373
	i64 u0x72e0300099accce1, ; 540: System.Xml.XPath.XDocument => 163
	i64 u0x730bfb248998f67a, ; 541: System.IO.Compression.ZipFile => 45
	i64 u0x732b2d67b9e5c47b, ; 542: Xamarin.Google.ErrorProne.Annotations.dll => 351
	i64 u0x734b76fdc0dc05bb, ; 543: lib_GoogleGson.dll.so => 185
	i64 u0x73a6be34e822f9d1, ; 544: lib_System.Runtime.Serialization.dll.so => 118
	i64 u0x73e4ce94e2eb6ffc, ; 545: lib_System.Memory.dll.so => 64
	i64 u0x743a1eccf080489a, ; 546: WindowsBase.dll => 169
	i64 u0x7465c42afc9ef57e, ; 547: Microsoft.AspNetCore.Identity.EntityFrameworkCore => 206
	i64 u0x746cf89b511b4d40, ; 548: lib_Microsoft.Extensions.Diagnostics.dll.so => 227
	i64 u0x755a91767330b3d4, ; 549: lib_Microsoft.Extensions.Configuration.dll.so => 221
	i64 u0x75c326eb821b85c4, ; 550: lib_System.ComponentModel.DataAnnotations.dll.so => 14
	i64 u0x76012e7334db86e5, ; 551: lib_Xamarin.AndroidX.SavedState.dll.so => 330
	i64 u0x76ca07b878f44da0, ; 552: System.Runtime.Numerics.dll => 113
	i64 u0x7736c8a96e51a061, ; 553: lib_Xamarin.AndroidX.Annotation.Jvm.dll.so => 277
	i64 u0x778a805e625329ef, ; 554: System.Linq.Parallel => 61
	i64 u0x77d9074d8f33a303, ; 555: lib_System.Net.ServerSentEvents.dll.so => 76
	i64 u0x77f8a4acc2fdc449, ; 556: System.Security.Cryptography.Cng.dll => 123
	i64 u0x780bc73597a503a9, ; 557: lib-ms-Microsoft.Maui.Controls.resources.dll.so => 379
	i64 u0x782c5d8eb99ff201, ; 558: lib_Microsoft.VisualBasic.Core.dll.so => 2
	i64 u0x783606d1e53e7a1a, ; 559: th/Microsoft.Maui.Controls.resources.dll => 389
	i64 u0x78a45e51311409b6, ; 560: Xamarin.AndroidX.Fragment.dll => 302
	i64 u0x78ed4ab8f9d800a1, ; 561: Xamarin.AndroidX.Lifecycle.ViewModel => 315
	i64 u0x7a25bdb29108c6e7, ; 562: Microsoft.Extensions.Http => 231
	i64 u0x7a5207a7c82d30b4, ; 563: lib_Xamarin.JSpecify.dll.so => 354
	i64 u0x7a7e7eddf79c5d26, ; 564: lib_Xamarin.AndroidX.Lifecycle.ViewModel.dll.so => 315
	i64 u0x7a9a57d43b0845fa, ; 565: System.AppContext => 6
	i64 u0x7ad0f4f1e5d08183, ; 566: Xamarin.AndroidX.Collection.dll => 284
	i64 u0x7adb8da2ac89b647, ; 567: fi/Microsoft.Maui.Controls.resources.dll => 369
	i64 u0x7b13d9eaa944ade8, ; 568: Xamarin.AndroidX.DynamicAnimation.dll => 298
	i64 u0x7b150145c0a9058c, ; 569: Microsoft.Data.Sqlite => 213
	i64 u0x7b4927e421291c41, ; 570: Microsoft.IdentityModel.JsonWebTokens.dll => 243
	i64 u0x7bef86a4335c4870, ; 571: System.ComponentModel.TypeConverter => 17
	i64 u0x7c0820144cd34d6a, ; 572: sk/Microsoft.Maui.Controls.resources.dll => 387
	i64 u0x7c2a0bd1e0f988fc, ; 573: lib-de-Microsoft.Maui.Controls.resources.dll.so => 366
	i64 u0x7c41d387501568ba, ; 574: System.Net.WebClient.dll => 79
	i64 u0x7c482cd79bd24b13, ; 575: lib_Xamarin.AndroidX.ConstraintLayout.dll.so => 288
	i64 u0x7c60acf6404e96b6, ; 576: Xamarin.AndroidX.Navigation.Common.Android.dll => 322
	i64 u0x7c8f4b4f3731320f, ; 577: Pipelines.Sockets.Unofficial.dll => 255
	i64 u0x7cc637f941f716d0, ; 578: CommunityToolkit.Maui.Core => 180
	i64 u0x7cd2ec8eaf5241cd, ; 579: System.Security.dll => 133
	i64 u0x7cf9ae50dd350622, ; 580: Xamarin.Jetbrains.Annotations.dll => 353
	i64 u0x7d4040680e64c3ea, ; 581: Pipelines.Sockets.Unofficial => 255
	i64 u0x7d649b75d580bb42, ; 582: ms/Microsoft.Maui.Controls.resources.dll => 379
	i64 u0x7d832b3e1fe0edbb, ; 583: Modules.Common.Grpc.Contacts.dll => 398
	i64 u0x7d8ee2bdc8e3aad1, ; 584: System.Numerics.Vectors => 85
	i64 u0x7df5df8db8eaa6ac, ; 585: Microsoft.Extensions.Logging.Debug => 236
	i64 u0x7dfc3d6d9d8d7b70, ; 586: System.Collections => 12
	i64 u0x7e2e564fa2f76c65, ; 587: lib_System.Diagnostics.Tracing.dll.so => 34
	i64 u0x7e302e110e1e1346, ; 588: lib_System.Security.Claims.dll.so => 121
	i64 u0x7e4084a672f9c30e, ; 589: lib_System.Security.Cryptography.Xml.dll.so => 268
	i64 u0x7e4465b3f78ad8d0, ; 590: Xamarin.KotlinX.Serialization.Core.dll => 359
	i64 u0x7e571cad5915e6c3, ; 591: lib_Xamarin.AndroidX.Lifecycle.Process.dll.so => 310
	i64 u0x7e6ac99e4e8df72f, ; 592: System.IO.Hashing => 176
	i64 u0x7e6b1ca712437d7d, ; 593: Xamarin.AndroidX.Emoji2.ViewsHelper => 300
	i64 u0x7e946809d6008ef2, ; 594: lib_System.ObjectModel.dll.so => 87
	i64 u0x7ea0272c1b4a9635, ; 595: lib_Xamarin.Android.Glide.dll.so => 269
	i64 u0x7ebe6126501e1198, ; 596: Microsoft.AspNetCore.Cryptography.KeyDerivation.dll => 196
	i64 u0x7ecc13347c8fd849, ; 597: lib_System.ComponentModel.dll.so => 18
	i64 u0x7eff369f2e01cf95, ; 598: Microsoft.AspNetCore.Http.Features => 204
	i64 u0x7f00ddd9b9ca5a13, ; 599: Xamarin.AndroidX.ViewPager.dll => 343
	i64 u0x7f9351cd44b1273f, ; 600: Microsoft.Extensions.Configuration.Abstractions => 222
	i64 u0x7fbd557c99b3ce6f, ; 601: lib_Xamarin.AndroidX.Lifecycle.LiveData.Core.dll.so => 308
	i64 u0x7fd75077141d6658, ; 602: Microsoft.AspNetCore.Authorization.Policy => 194
	i64 u0x8076a9a44a2ca331, ; 603: System.Net.Quic => 73
	i64 u0x80da183a87731838, ; 604: System.Reflection.Metadata => 97
	i64 u0x80fa55b6d1b0be99, ; 605: SQLitePCLRaw.provider.e_sqlite3 => 262
	i64 u0x812c069d5cdecc17, ; 606: System.dll => 168
	i64 u0x81381be520a60adb, ; 607: Xamarin.AndroidX.Interpolator.dll => 304
	i64 u0x81657cec2b31e8aa, ; 608: System.Net => 84
	i64 u0x81ab745f6c0f5ce6, ; 609: zh-Hant/Microsoft.Maui.Controls.resources => 395
	i64 u0x822aa49008112ebe, ; 610: Microsoft.Extensions.ObjectPool => 237
	i64 u0x8235b6241d2f648b, ; 611: lib_Modules.Users.Domain.dll.so => 400
	i64 u0x8277f2be6b5ce05f, ; 612: Xamarin.AndroidX.AppCompat => 278
	i64 u0x828f06563b30bc50, ; 613: lib_Xamarin.AndroidX.CardView.dll.so => 283
	i64 u0x82b399cb01b531c4, ; 614: lib_System.Web.dll.so => 157
	i64 u0x82df8f5532a10c59, ; 615: lib_System.Drawing.dll.so => 36
	i64 u0x82f0b6e911d13535, ; 616: lib_System.Transactions.dll.so => 154
	i64 u0x82f6403342e12049, ; 617: uk/Microsoft.Maui.Controls.resources => 391
	i64 u0x83144699b312ad81, ; 618: SQLite-net.dll => 258
	i64 u0x83a7afd2c49adc86, ; 619: lib_Microsoft.IdentityModel.Abstractions.dll.so => 242
	i64 u0x83c14ba66c8e2b8c, ; 620: zh-Hans/Microsoft.Maui.Controls.resources => 394
	i64 u0x846ce984efea52c7, ; 621: System.Threading.Tasks.Parallel.dll => 147
	i64 u0x84ae73148a4557d2, ; 622: lib_System.IO.Pipes.dll.so => 56
	i64 u0x84b01102c12a9232, ; 623: System.Runtime.Serialization.Json.dll => 115
	i64 u0x84cd5cdec0f54bcc, ; 624: lib_Microsoft.EntityFrameworkCore.Relational.dll.so => 217
	i64 u0x84f20950c4c7164b, ; 625: Microsoft.AspNetCore.Http => 201
	i64 u0x850c5ba0b57ce8e7, ; 626: lib_Xamarin.AndroidX.Collection.dll.so => 284
	i64 u0x851d02edd334b044, ; 627: Xamarin.AndroidX.VectorDrawable => 340
	i64 u0x85c919db62150978, ; 628: Xamarin.AndroidX.Transition.dll => 339
	i64 u0x8662aaeb94fef37f, ; 629: lib_System.Dynamic.Runtime.dll.so => 37
	i64 u0x86a909228dc7657b, ; 630: lib-zh-Hant-Microsoft.Maui.Controls.resources.dll.so => 395
	i64 u0x86b3e00c36b84509, ; 631: Microsoft.Extensions.Configuration.dll => 221
	i64 u0x86b62cb077ec4fd7, ; 632: System.Runtime.Serialization.Xml => 117
	i64 u0x8706ffb12bf3f53d, ; 633: Xamarin.AndroidX.Annotation.Experimental => 276
	i64 u0x872a5b14c18d328c, ; 634: System.ComponentModel.DataAnnotations => 14
	i64 u0x872fb9615bc2dff0, ; 635: Xamarin.Android.Glide.Annotations.dll => 270
	i64 u0x87c4b8a492b176ad, ; 636: Microsoft.EntityFrameworkCore.Abstractions => 216
	i64 u0x87c69b87d9283884, ; 637: lib_System.Threading.Thread.dll.so => 149
	i64 u0x87d6cb5c641c5f07, ; 638: Microsoft.AspNetCore.Http.Abstractions.dll => 202
	i64 u0x87f6569b25707834, ; 639: System.IO.Compression.Brotli.dll => 43
	i64 u0x87fef727071b7fe5, ; 640: Grpc.Net.Client => 187
	i64 u0x8842b3a5d2d3fb36, ; 641: Microsoft.Maui.Essentials => 249
	i64 u0x88926583efe7ee86, ; 642: Xamarin.AndroidX.Activity.Ktx.dll => 274
	i64 u0x88ba6bc4f7762b03, ; 643: lib_System.Reflection.dll.so => 100
	i64 u0x88bda98e0cffb7a9, ; 644: lib_Xamarin.KotlinX.Coroutines.Core.Jvm.dll.so => 358
	i64 u0x88f629147ff1577f, ; 645: lib_Confluent.Kafka.dll.so => 182
	i64 u0x8930322c7bd8f768, ; 646: netstandard => 171
	i64 u0x897a606c9e39c75f, ; 647: lib_System.ComponentModel.Primitives.dll.so => 16
	i64 u0x898a5c6bc9e47ec1, ; 648: lib_Xamarin.AndroidX.SavedState.SavedState.Android.dll.so => 331
	i64 u0x898a9b4e63f2c138, ; 649: lib_Microsoft.AspNetCore.Identity.dll.so => 205
	i64 u0x89911a22005b92b7, ; 650: System.IO.FileSystem.DriveInfo.dll => 48
	i64 u0x89c5188089ec2cd5, ; 651: lib_System.Runtime.InteropServices.RuntimeInformation.dll.so => 109
	i64 u0x8a0b6f586fccda8a, ; 652: lib_Microsoft.AspNetCore.Http.Extensions.dll.so => 203
	i64 u0x8a14bf4400a024af, ; 653: lib_Microsoft.AspNetCore.Http.Features.dll.so => 204
	i64 u0x8a19e3dc71b34b2c, ; 654: System.Reflection.TypeExtensions.dll => 99
	i64 u0x8a399a706fcbce4b, ; 655: Microsoft.Extensions.Caching.Abstractions => 218
	i64 u0x8ad229ea26432ee2, ; 656: Xamarin.AndroidX.Loader => 320
	i64 u0x8b1b7008acd6fcc7, ; 657: Modules.Users.Infrastructure => 402
	i64 u0x8b4ff5d0fdd5faa1, ; 658: lib_System.Diagnostics.DiagnosticSource.dll.so => 27
	i64 u0x8b541d476eb3774c, ; 659: System.Security.Principal.Windows => 130
	i64 u0x8b8d01333a96d0b5, ; 660: System.Diagnostics.Process.dll => 29
	i64 u0x8b9ceca7acae3451, ; 661: lib-he-Microsoft.Maui.Controls.resources.dll.so => 371
	i64 u0x8bb8206f414c7c3b, ; 662: Microsoft.AspNetCore.Authentication.Core.dll => 192
	i64 u0x8c575135aa1ccef4, ; 663: Microsoft.Extensions.FileProviders.Abstractions => 229
	i64 u0x8cb8f612b633affb, ; 664: Xamarin.AndroidX.SavedState.SavedState.Ktx.dll => 332
	i64 u0x8cdfdb4ce85fb925, ; 665: lib_System.Security.Principal.Windows.dll.so => 130
	i64 u0x8cdfe7b8f4caa426, ; 666: System.IO.Compression.FileSystem => 44
	i64 u0x8d0f420977c2c1c7, ; 667: Xamarin.AndroidX.CursorAdapter.dll => 294
	i64 u0x8d52f7ea2796c531, ; 668: Xamarin.AndroidX.Emoji2.dll => 299
	i64 u0x8d7b8ab4b3310ead, ; 669: System.Threading => 152
	i64 u0x8da188285aadfe8e, ; 670: System.Collections.Concurrent => 8
	i64 u0x8dce248c34c54ef3, ; 671: lib_Microsoft.AspNetCore.Hosting.Abstractions.dll.so => 199
	i64 u0x8dfc1cfbf8858f95, ; 672: Grpc.Core.Api.dll => 186
	i64 u0x8e623fec9635e28f, ; 673: Syncfusion.Maui.Toolkit.resources.dll => 265
	i64 u0x8e8f269ad1e1ff94, ; 674: lib_Xamarin.AndroidX.Tracing.Tracing.Android.dll.so => 338
	i64 u0x8ec6e06a61c1baeb, ; 675: lib_Newtonsoft.Json.dll.so => 252
	i64 u0x8ed807bfe9858dfc, ; 676: Xamarin.AndroidX.Navigation.Common => 321
	i64 u0x8ee08b8194a30f48, ; 677: lib-hi-Microsoft.Maui.Controls.resources.dll.so => 372
	i64 u0x8ef7601039857a44, ; 678: lib-ro-Microsoft.Maui.Controls.resources.dll.so => 385
	i64 u0x8ef9414937d93a0a, ; 679: SQLitePCLRaw.core.dll => 260
	i64 u0x8f32c6f611f6ffab, ; 680: pt/Microsoft.Maui.Controls.resources.dll => 384
	i64 u0x8f44b45eb046bbd1, ; 681: System.ServiceModel.Web.dll => 134
	i64 u0x8f8829d21c8985a4, ; 682: lib-pt-BR-Microsoft.Maui.Controls.resources.dll.so => 383
	i64 u0x8f97020698a101ba, ; 683: Microsoft.AspNetCore.Routing.dll => 210
	i64 u0x8fbf5b0114c6dcef, ; 684: System.Globalization.dll => 42
	i64 u0x8fcc8c2a81f3d9e7, ; 685: Xamarin.KotlinX.Serialization.Core => 359
	i64 u0x8fd27d934d7b3a55, ; 686: SQLitePCLRaw.core => 260
	i64 u0x90263f8448b8f572, ; 687: lib_System.Diagnostics.TraceSource.dll.so => 33
	i64 u0x90281820febeff00, ; 688: lib_Microsoft.AspNetCore.Routing.Abstractions.dll.so => 211
	i64 u0x903101b46fb73a04, ; 689: _Microsoft.Android.Resource.Designer => 403
	i64 u0x90393bd4865292f3, ; 690: lib_System.IO.Compression.dll.so => 46
	i64 u0x905e2b8e7ae91ae6, ; 691: System.Threading.Tasks.Extensions.dll => 146
	i64 u0x90634f86c5ebe2b5, ; 692: Xamarin.AndroidX.Lifecycle.ViewModel.Android => 316
	i64 u0x907b636704ad79ef, ; 693: lib_Microsoft.Maui.Controls.Xaml.dll.so => 247
	i64 u0x90e9efbfd68593e0, ; 694: lib_Xamarin.AndroidX.Lifecycle.LiveData.dll.so => 307
	i64 u0x91418dc638b29e68, ; 695: lib_Xamarin.AndroidX.CustomView.dll.so => 295
	i64 u0x9157bd523cd7ed36, ; 696: lib_System.Text.Json.dll.so => 140
	i64 u0x91a74f07b30d37e2, ; 697: System.Linq.dll => 63
	i64 u0x91cb86ea3b17111d, ; 698: System.ServiceModel.Web => 134
	i64 u0x91fa41a87223399f, ; 699: ca/Microsoft.Maui.Controls.resources.dll => 363
	i64 u0x92054e486c0c7ea7, ; 700: System.IO.FileSystem.DriveInfo => 48
	i64 u0x928614058c40c4cd, ; 701: lib_System.Xml.XPath.XDocument.dll.so => 163
	i64 u0x92b138fffca2b01e, ; 702: lib_Xamarin.AndroidX.Arch.Core.Runtime.dll.so => 281
	i64 u0x92dd6c6033393bf7, ; 703: Syncfusion.Maui.Toolkit.resources => 265
	i64 u0x92dfc2bfc6c6a888, ; 704: Xamarin.AndroidX.Lifecycle.LiveData => 307
	i64 u0x933da2c779423d68, ; 705: Xamarin.Android.Glide.Annotations => 270
	i64 u0x9347fbe3e99955dd, ; 706: YamlDotNet.dll => 361
	i64 u0x9388aad9b7ae40ce, ; 707: lib_Xamarin.AndroidX.Lifecycle.Common.dll.so => 305
	i64 u0x93cfa73ab28d6e35, ; 708: ms/Microsoft.Maui.Controls.resources => 379
	i64 u0x941c00d21e5c0679, ; 709: lib_Xamarin.AndroidX.Transition.dll.so => 339
	i64 u0x942e2b32e944adec, ; 710: Modules.Users.Domain.dll => 400
	i64 u0x944077d8ca3c6580, ; 711: System.IO.Compression.dll => 46
	i64 u0x948cffedc8ed7960, ; 712: System.Xml => 167
	i64 u0x948d746a7702861f, ; 713: Microsoft.IdentityModel.Logging.dll => 244
	i64 u0x94bbeab0d4764588, ; 714: System.IO.Hashing.dll => 176
	i64 u0x94c8990839c4bdb1, ; 715: lib_Xamarin.AndroidX.Interpolator.dll.so => 304
	i64 u0x9564283c37ed59a9, ; 716: lib_Microsoft.IdentityModel.Logging.dll.so => 244
	i64 u0x957a4cdfdcfd6d83, ; 717: Refit.HttpClientFactory.dll => 257
	i64 u0x967fc325e09bfa8c, ; 718: es/Microsoft.Maui.Controls.resources => 368
	i64 u0x9686161486d34b81, ; 719: lib_Xamarin.AndroidX.ExifInterface.dll.so => 301
	i64 u0x9732d8dbddea3d9a, ; 720: id/Microsoft.Maui.Controls.resources => 375
	i64 u0x978be80e5210d31b, ; 721: Microsoft.Maui.Graphics.dll => 250
	i64 u0x97b8c771ea3e4220, ; 722: System.ComponentModel.dll => 18
	i64 u0x97e144c9d3c6976e, ; 723: System.Collections.Concurrent.dll => 8
	i64 u0x984184e3c70d4419, ; 724: GoogleGson => 185
	i64 u0x9843944103683dd3, ; 725: Xamarin.AndroidX.Core.Core.Ktx => 292
	i64 u0x9855609d6d191a4b, ; 726: lib_Modules.Common.Grpc.Contacts.dll.so => 398
	i64 u0x98b05cc81e6f333c, ; 727: Xamarin.AndroidX.SavedState.SavedState.Android.dll => 331
	i64 u0x98d720cc4597562c, ; 728: System.Security.Cryptography.OpenSsl => 126
	i64 u0x991d510397f92d9d, ; 729: System.Linq.Expressions => 60
	i64 u0x996ceeb8a3da3d67, ; 730: System.Threading.Overlapped.dll => 144
	i64 u0x999cb19e1a04ffd3, ; 731: CommunityToolkit.Mvvm.dll => 181
	i64 u0x99a00ca5270c6878, ; 732: Xamarin.AndroidX.Navigation.Runtime => 324
	i64 u0x99cdc6d1f2d3a72f, ; 733: ko/Microsoft.Maui.Controls.resources.dll => 378
	i64 u0x9a01b1da98b6ee10, ; 734: Xamarin.AndroidX.Lifecycle.Runtime.dll => 311
	i64 u0x9a5ccc274fd6e6ee, ; 735: Jsr305Binding.dll => 349
	i64 u0x9acfd25e735d5594, ; 736: lib_Npgsql.dll.so => 253
	i64 u0x9ae6940b11c02876, ; 737: lib_Xamarin.AndroidX.Window.dll.so => 345
	i64 u0x9b211a749105beac, ; 738: System.Transactions.Local => 153
	i64 u0x9b8734714671022d, ; 739: System.Threading.Tasks.Dataflow.dll => 145
	i64 u0x9bc6aea27fbf034f, ; 740: lib_Xamarin.KotlinX.Coroutines.Core.dll.so => 357
	i64 u0x9c244ac7cda32d26, ; 741: System.Security.Cryptography.X509Certificates.dll => 128
	i64 u0x9c465f280cf43733, ; 742: lib_Xamarin.KotlinX.Coroutines.Android.dll.so => 356
	i64 u0x9c6a130862518b21, ; 743: Modules.Common.Domain => 397
	i64 u0x9c8f6872beab6408, ; 744: System.Xml.XPath.XDocument.dll => 163
	i64 u0x9ce01cf91101ae23, ; 745: System.Xml.XmlDocument => 165
	i64 u0x9d128180c81d7ce6, ; 746: Xamarin.AndroidX.CustomView.PoolingContainer => 296
	i64 u0x9d5dbcf5a48583fe, ; 747: lib_Xamarin.AndroidX.Activity.dll.so => 273
	i64 u0x9d74dee1a7725f34, ; 748: Microsoft.Extensions.Configuration.Abstractions.dll => 222
	i64 u0x9dd0e195825d65c6, ; 749: lib_Xamarin.AndroidX.Navigation.Runtime.Android.dll.so => 325
	i64 u0x9e4534b6adaf6e84, ; 750: nl/Microsoft.Maui.Controls.resources => 381
	i64 u0x9e4b95dec42769f7, ; 751: System.Diagnostics.Debug.dll => 26
	i64 u0x9eaf1efdf6f7267e, ; 752: Xamarin.AndroidX.Navigation.Common.dll => 321
	i64 u0x9ef542cf1f78c506, ; 753: Xamarin.AndroidX.Lifecycle.LiveData.Core => 308
	i64 u0x9ffc74b9e35af6c6, ; 754: Modules.Users.DTO.dll => 401
	i64 u0xa00832eb975f56a8, ; 755: lib_System.Net.dll.so => 84
	i64 u0xa06617c0e4916b8f, ; 756: Microsoft.Extensions.Caching.StackExchangeRedis.dll => 220
	i64 u0xa0ad78236b7b267f, ; 757: Xamarin.AndroidX.Window => 345
	i64 u0xa0d8259f4cc284ec, ; 758: lib_System.Security.Cryptography.dll.so => 129
	i64 u0xa0e17ca50c77a225, ; 759: lib_Xamarin.Google.Crypto.Tink.Android.dll.so => 350
	i64 u0xa0ff9b3e34d92f11, ; 760: lib_System.Resources.Writer.dll.so => 103
	i64 u0xa12fbfb4da97d9f3, ; 761: System.Threading.Timer.dll => 151
	i64 u0xa13f33d9a41bee22, ; 762: FluentValidation.dll => 183
	i64 u0xa1440773ee9d341e, ; 763: Xamarin.Google.Android.Material => 348
	i64 u0xa18c39c44cdc3465, ; 764: Xamarin.AndroidX.Window.WindowCore => 346
	i64 u0xa1b9d7c27f47219f, ; 765: Xamarin.AndroidX.Navigation.UI.dll => 326
	i64 u0xa1cfec8d4a8d7c32, ; 766: Npgsql.EntityFrameworkCore.PostgreSQL.dll => 254
	i64 u0xa2572680829d2c7c, ; 767: System.IO.Pipelines.dll => 54
	i64 u0xa26597e57ee9c7f6, ; 768: System.Xml.XmlDocument.dll => 165
	i64 u0xa308401900e5bed3, ; 769: lib_mscorlib.dll.so => 170
	i64 u0xa395572e7da6c99d, ; 770: lib_System.Security.dll.so => 133
	i64 u0xa3c64c49e90a9987, ; 771: System.Security.Cryptography.Pkcs => 267
	i64 u0xa3e683f24b43af6f, ; 772: System.Dynamic.Runtime.dll => 37
	i64 u0xa4145becdee3dc4f, ; 773: Xamarin.AndroidX.VectorDrawable.Animated => 341
	i64 u0xa46aa1eaa214539b, ; 774: ko/Microsoft.Maui.Controls.resources => 378
	i64 u0xa473938c792db0b6, ; 775: Microsoft.AspNetCore.Routing.Abstractions => 211
	i64 u0xa4a372eecb9e4df0, ; 776: Microsoft.Extensions.Diagnostics => 227
	i64 u0xa4d20d2ff0563d26, ; 777: lib_CommunityToolkit.Mvvm.dll.so => 181
	i64 u0xa4edc8f2ceae241a, ; 778: System.Data.Common.dll => 22
	i64 u0xa5494f40f128ce6a, ; 779: System.Runtime.Serialization.Formatters.dll => 114
	i64 u0xa54b74df83dce92b, ; 780: System.Reflection.DispatchProxy => 92
	i64 u0xa5b7152421ed6d98, ; 781: lib_System.IO.FileSystem.Watcher.dll.so => 50
	i64 u0xa5c3844f17b822db, ; 782: lib_System.Linq.Parallel.dll.so => 61
	i64 u0xa5ce5c755bde8cb8, ; 783: lib_System.Security.Cryptography.Csp.dll.so => 124
	i64 u0xa5e599d1e0524750, ; 784: System.Numerics.Vectors.dll => 85
	i64 u0xa5f1ba49b85dd355, ; 785: System.Security.Cryptography.dll => 129
	i64 u0xa60fdaa9af524b6a, ; 786: Microsoft.DotNet.PlatformAbstractions.dll => 214
	i64 u0xa61975a5a37873ea, ; 787: lib_System.Xml.XmlSerializer.dll.so => 166
	i64 u0xa6593e21584384d2, ; 788: lib_Jsr305Binding.dll.so => 349
	i64 u0xa66cbee0130865f7, ; 789: lib_WindowsBase.dll.so => 169
	i64 u0xa67dbee13e1df9ca, ; 790: Xamarin.AndroidX.SavedState.dll => 330
	i64 u0xa684b098dd27b296, ; 791: lib_Xamarin.AndroidX.Security.SecurityCrypto.dll.so => 333
	i64 u0xa68a420042bb9b1f, ; 792: Xamarin.AndroidX.DrawerLayout.dll => 297
	i64 u0xa6d26156d1cacc7c, ; 793: Xamarin.Android.Glide.dll => 269
	i64 u0xa75386b5cb9595aa, ; 794: Xamarin.AndroidX.Lifecycle.Runtime.Android => 312
	i64 u0xa75cf331ee476318, ; 795: lib_Microsoft.AspNetCore.Http.Abstractions.dll.so => 202
	i64 u0xa763fbb98df8d9fb, ; 796: lib_Microsoft.Win32.Primitives.dll.so => 4
	i64 u0xa78ce3745383236a, ; 797: Xamarin.AndroidX.Lifecycle.Common.Jvm => 306
	i64 u0xa7c31b56b4dc7b33, ; 798: hu/Microsoft.Maui.Controls.resources => 374
	i64 u0xa7eab29ed44b4e7a, ; 799: Mono.Android.Export => 173
	i64 u0xa8195217cbf017b7, ; 800: Microsoft.VisualBasic.Core => 2
	i64 u0xa859a95830f367ff, ; 801: lib_Xamarin.AndroidX.Lifecycle.ViewModel.Ktx.dll.so => 317
	i64 u0xa8b52f21e0dbe690, ; 802: System.Runtime.Serialization.dll => 118
	i64 u0xa8bb61a1ac3eba47, ; 803: lib_Modules.Common.Infrastructure.dll.so => 399
	i64 u0xa8e6320dd07580ef, ; 804: lib_Microsoft.IdentityModel.JsonWebTokens.dll.so => 243
	i64 u0xa8ee4ed7de2efaee, ; 805: Xamarin.AndroidX.Annotation.dll => 275
	i64 u0xa95590e7c57438a4, ; 806: System.Configuration => 19
	i64 u0xa964304b5631e28a, ; 807: CommunityToolkit.Maui.Core.dll => 180
	i64 u0xaa2219c8e3449ff5, ; 808: Microsoft.Extensions.Logging.Abstractions => 235
	i64 u0xaa443ac34067eeef, ; 809: System.Private.Xml.dll => 91
	i64 u0xaa52de307ef5d1dd, ; 810: System.Net.Http => 66
	i64 u0xaa9a7b0214a5cc5c, ; 811: System.Diagnostics.StackTrace.dll => 30
	i64 u0xaaaf86367285a918, ; 812: Microsoft.Extensions.DependencyInjection.Abstractions.dll => 225
	i64 u0xaaf84bb3f052a265, ; 813: el/Microsoft.Maui.Controls.resources => 367
	i64 u0xab9af77b5b67a0b8, ; 814: Xamarin.AndroidX.ConstraintLayout.Core => 289
	i64 u0xab9c1b2687d86b0b, ; 815: lib_System.Linq.Expressions.dll.so => 60
	i64 u0xabaabcb70f9474ff, ; 816: Microsoft.AspNetCore.Mvc.Abstractions => 207
	i64 u0xac2af3fa195a15ce, ; 817: System.Runtime.Numerics => 113
	i64 u0xac5376a2a538dc10, ; 818: Xamarin.AndroidX.Lifecycle.LiveData.Core.dll => 308
	i64 u0xac5acae88f60357e, ; 819: System.Diagnostics.Tools.dll => 32
	i64 u0xac65e40f62b6b90e, ; 820: Google.Protobuf => 184
	i64 u0xac79c7e46047ad98, ; 821: System.Security.Principal.Windows.dll => 130
	i64 u0xac98d31068e24591, ; 822: System.Xml.XDocument => 162
	i64 u0xacd46e002c3ccb97, ; 823: ro/Microsoft.Maui.Controls.resources => 385
	i64 u0xacd4f3866b293bb7, ; 824: Microsoft.AspNetCore.Authentication.Cookies => 191
	i64 u0xacdd9e4180d56dda, ; 825: Xamarin.AndroidX.Concurrent.Futures => 287
	i64 u0xacf42eea7ef9cd12, ; 826: System.Threading.Channels => 143
	i64 u0xad89c07347f1bad6, ; 827: nl/Microsoft.Maui.Controls.resources.dll => 381
	i64 u0xadbb53caf78a79d2, ; 828: System.Web.HttpUtility => 156
	i64 u0xadc90ab061a9e6e4, ; 829: System.ComponentModel.TypeConverter.dll => 17
	i64 u0xadca1b9030b9317e, ; 830: Xamarin.AndroidX.Collection.Ktx => 286
	i64 u0xadd8eda2edf396ad, ; 831: Xamarin.Android.Glide.GifDecoder => 272
	i64 u0xadf4cf30debbeb9a, ; 832: System.Net.ServicePoint.dll => 77
	i64 u0xadf511667bef3595, ; 833: System.Net.Security => 75
	i64 u0xae0aaa94fdcfce0f, ; 834: System.ComponentModel.EventBasedAsync.dll => 15
	i64 u0xae282bcd03739de7, ; 835: Java.Interop => 172
	i64 u0xae53579c90db1107, ; 836: System.ObjectModel.dll => 87
	i64 u0xae7ea18c61eef394, ; 837: SQLite-net => 258
	i64 u0xaf4829c0b3e740ae, ; 838: lib_Syncfusion.Maui.Toolkit.resources.dll.so => 265
	i64 u0xaf732d0b2193b8f5, ; 839: System.Security.Cryptography.OpenSsl.dll => 126
	i64 u0xafdb94dbccd9d11c, ; 840: Xamarin.AndroidX.Lifecycle.LiveData.dll => 307
	i64 u0xafe29f45095518e7, ; 841: lib_Xamarin.AndroidX.Lifecycle.ViewModelSavedState.dll.so => 318
	i64 u0xb03ae931fb25607e, ; 842: Xamarin.AndroidX.ConstraintLayout => 288
	i64 u0xb05cc42cd94c6d9d, ; 843: lib-sv-Microsoft.Maui.Controls.resources.dll.so => 388
	i64 u0xb0ac21bec8f428c5, ; 844: Xamarin.AndroidX.Lifecycle.Runtime.Ktx.Android.dll => 314
	i64 u0xb0bb43dc52ea59f9, ; 845: System.Diagnostics.Tracing.dll => 34
	i64 u0xb110d64b6c9fbe46, ; 846: lib_Microsoft.Extensions.Identity.Core.dll.so => 232
	i64 u0xb1dd05401aa8ee63, ; 847: System.Security.AccessControl => 120
	i64 u0xb220631954820169, ; 848: System.Text.RegularExpressions => 141
	i64 u0xb2376e1dbf8b4ed7, ; 849: System.Security.Cryptography.Csp => 124
	i64 u0xb2a1959fe95c5402, ; 850: lib_System.Runtime.InteropServices.JavaScript.dll.so => 108
	i64 u0xb2a3f67f3bf29fce, ; 851: da/Microsoft.Maui.Controls.resources => 365
	i64 u0xb3874072ee0ecf8c, ; 852: Xamarin.AndroidX.VectorDrawable.Animated.dll => 341
	i64 u0xb3f0a0fcda8d3ebc, ; 853: Xamarin.AndroidX.CardView => 283
	i64 u0xb456c3b417a382f5, ; 854: lib_AKSoftware.Localization.MultiLanguages.dll.so => 178
	i64 u0xb46be1aa6d4fff93, ; 855: hi/Microsoft.Maui.Controls.resources => 372
	i64 u0xb477491be13109d8, ; 856: ar/Microsoft.Maui.Controls.resources => 362
	i64 u0xb4ba8ad4cb954eb3, ; 857: Modules.Common.Infrastructure => 399
	i64 u0xb4bd7015ecee9d86, ; 858: System.IO.Pipelines => 54
	i64 u0xb4c53d9749c5f226, ; 859: lib_System.IO.FileSystem.AccessControl.dll.so => 47
	i64 u0xb4ff710863453fda, ; 860: System.Diagnostics.FileVersionInfo.dll => 28
	i64 u0xb50d9ae4eea71e97, ; 861: lib_Microsoft.DotNet.PlatformAbstractions.dll.so => 214
	i64 u0xb52aa297a3a175b1, ; 862: lib_Microsoft.AspNetCore.Authentication.Core.dll.so => 192
	i64 u0xb54092076b15e062, ; 863: System.Threading.AccessControl => 142
	i64 u0xb545f78b0415b9b9, ; 864: Microsoft.AspNetCore.WebUtilities.dll => 212
	i64 u0xb5c38bf497a4cfe2, ; 865: lib_System.Threading.Tasks.dll.so => 148
	i64 u0xb5c7fcdafbc67ee4, ; 866: Microsoft.Extensions.Logging.Abstractions.dll => 235
	i64 u0xb5dc0290c441c648, ; 867: lib_Microsoft.AspNetCore.Authentication.Cookies.dll.so => 191
	i64 u0xb5e59badb43e7829, ; 868: Microsoft.AspNetCore.Routing.Abstractions.dll => 211
	i64 u0xb5ea31d5244c6626, ; 869: System.Threading.ThreadPool.dll => 150
	i64 u0xb7212c4683a94afe, ; 870: System.Drawing.Primitives => 35
	i64 u0xb7b7753d1f319409, ; 871: sv/Microsoft.Maui.Controls.resources => 388
	i64 u0xb7fb824ee514af57, ; 872: Modules.Common.Domain.dll => 397
	i64 u0xb81a2c6e0aee50fe, ; 873: lib_System.Private.CoreLib.dll.so => 177
	i64 u0xb872c26142d22aa9, ; 874: Microsoft.Extensions.Http.dll => 231
	i64 u0xb8c60af47c08d4da, ; 875: System.Net.ServicePoint => 77
	i64 u0xb8e68d20aad91196, ; 876: lib_System.Xml.XPath.dll.so => 164
	i64 u0xb90ff82c284e9af9, ; 877: Grpc.Core.Api => 186
	i64 u0xb9185c33a1643eed, ; 878: Microsoft.CSharp.dll => 1
	i64 u0xb95c522c772254d2, ; 879: Microsoft.AspNetCore.DataProtection.dll => 197
	i64 u0xb960d6b2200ba320, ; 880: Xamarin.AndroidX.Lifecycle.ViewModelSavedState.Android.dll => 319
	i64 u0xb9b8001adf4ed7cc, ; 881: lib_Xamarin.AndroidX.SlidingPaneLayout.dll.so => 334
	i64 u0xb9f64d3b230def68, ; 882: lib-pt-Microsoft.Maui.Controls.resources.dll.so => 384
	i64 u0xb9fc3c8a556e3691, ; 883: ja/Microsoft.Maui.Controls.resources => 377
	i64 u0xba4670aa94a2b3c6, ; 884: lib_System.Xml.XDocument.dll.so => 162
	i64 u0xba48785529705af9, ; 885: System.Collections.dll => 12
	i64 u0xba965b8c86359996, ; 886: lib_System.Windows.dll.so => 158
	i64 u0xbb286883bc35db36, ; 887: System.Transactions.dll => 154
	i64 u0xbb639e0337b3d979, ; 888: Microsoft.AspNetCore.Http.dll => 201
	i64 u0xbb65706fde942ce3, ; 889: System.Net.Sockets => 78
	i64 u0xbba28979413cad9e, ; 890: lib_System.Runtime.CompilerServices.VisualC.dll.so => 105
	i64 u0xbbd180354b67271a, ; 891: System.Runtime.Serialization.Formatters => 114
	i64 u0xbbd599e40ecbe2a6, ; 892: Modules.Common.Infrastructure.dll => 399
	i64 u0xbc22a245dab70cb4, ; 893: lib_SQLitePCLRaw.provider.e_sqlite3.dll.so => 262
	i64 u0xbc260cdba33291a3, ; 894: Xamarin.AndroidX.Arch.Core.Common.dll => 280
	i64 u0xbcd36316d29f27b4, ; 895: lib_Microsoft.AspNetCore.Authorization.dll.so => 193
	i64 u0xbd0e2c0d55246576, ; 896: System.Net.Http.dll => 66
	i64 u0xbd3fbd85b9e1cb29, ; 897: lib_System.Net.HttpListener.dll.so => 67
	i64 u0xbd437a2cdb333d0d, ; 898: Xamarin.AndroidX.ViewPager2 => 344
	i64 u0xbd4f572d2bd0a789, ; 899: System.IO.Compression.ZipFile.dll => 45
	i64 u0xbd5d0b88d3d647a5, ; 900: lib_Xamarin.AndroidX.Browser.dll.so => 282
	i64 u0xbd770a375f100c23, ; 901: lib_Pipelines.Sockets.Unofficial.dll.so => 255
	i64 u0xbd877b14d0b56392, ; 902: System.Runtime.Intrinsics.dll => 111
	i64 u0xbde4cd9bb9008cb3, ; 903: lib_Microsoft.AspNetCore.Authentication.Abstractions.dll.so => 190
	i64 u0xbe4450ecf4d84c63, ; 904: lib_Microsoft.AspNetCore.ResponseCaching.Abstractions.dll.so => 209
	i64 u0xbe65a49036345cf4, ; 905: lib_System.Buffers.dll.so => 7
	i64 u0xbee38d4a88835966, ; 906: Xamarin.AndroidX.AppCompat.AppCompatResources => 279
	i64 u0xbef9919db45b4ca7, ; 907: System.IO.Pipes.AccessControl => 55
	i64 u0xbf0fa68611139208, ; 908: lib_Xamarin.AndroidX.Annotation.dll.so => 275
	i64 u0xbf677a56a0f14616, ; 909: Microsoft.AspNetCore.Authentication => 189
	i64 u0xbfc1e1fb3095f2b3, ; 910: lib_System.Net.Http.Json.dll.so => 65
	i64 u0xc040a4ab55817f58, ; 911: ar/Microsoft.Maui.Controls.resources.dll => 362
	i64 u0xc07cadab29efeba0, ; 912: Xamarin.AndroidX.Core.Core.Ktx.dll => 292
	i64 u0xc0d928351ab5ca77, ; 913: System.Console.dll => 20
	i64 u0xc0f5a221a9383aea, ; 914: System.Runtime.Intrinsics => 111
	i64 u0xc111030af54d7191, ; 915: System.Resources.Writer => 103
	i64 u0xc12b8b3afa48329c, ; 916: lib_System.Linq.dll.so => 63
	i64 u0xc1347413e524ff69, ; 917: lib_Syncfusion.Maui.Toolkit.dll.so => 264
	i64 u0xc183ca0b74453aa9, ; 918: lib_System.Threading.Tasks.Dataflow.dll.so => 145
	i64 u0xc1c2cb7af77b8858, ; 919: Microsoft.EntityFrameworkCore => 215
	i64 u0xc1ebdc7e6a943450, ; 920: Microsoft.AspNetCore.Authorization.dll => 193
	i64 u0xc1ff9ae3cdb6e1e6, ; 921: Xamarin.AndroidX.Activity.dll => 273
	i64 u0xc2654c6e949f22d9, ; 922: Microsoft.AspNetCore.Identity.EntityFrameworkCore.dll => 206
	i64 u0xc26c064effb1dea9, ; 923: System.Buffers.dll => 7
	i64 u0xc278de356ad8a9e3, ; 924: Microsoft.IdentityModel.Logging => 244
	i64 u0xc27e35acb993bc55, ; 925: Microsoft.AspNetCore.Identity.dll => 205
	i64 u0xc28c50f32f81cc73, ; 926: ja/Microsoft.Maui.Controls.resources.dll => 377
	i64 u0xc2902f6cf5452577, ; 927: lib_Mono.Android.Export.dll.so => 173
	i64 u0xc2a3bca55b573141, ; 928: System.IO.FileSystem.Watcher => 50
	i64 u0xc2bcfec99f69365e, ; 929: Xamarin.AndroidX.ViewPager2.dll => 344
	i64 u0xc30b52815b58ac2c, ; 930: lib_System.Runtime.Serialization.Xml.dll.so => 117
	i64 u0xc3492f8f90f96ce4, ; 931: lib_Microsoft.Extensions.DependencyModel.dll.so => 226
	i64 u0xc36d7d89c652f455, ; 932: System.Threading.Overlapped => 144
	i64 u0xc36f05e91f701d56, ; 933: lib_Modules.Users.DTO.dll.so => 401
	i64 u0xc374571bc2b0b5f8, ; 934: lib_Microsoft.AspNetCore.Routing.dll.so => 210
	i64 u0xc38fed85a861afa9, ; 935: Microsoft.Extensions.Caching.StackExchangeRedis => 220
	i64 u0xc396b285e59e5493, ; 936: GoogleGson.dll => 185
	i64 u0xc39ced8467203460, ; 937: lib_Refit.HttpClientFactory.dll.so => 257
	i64 u0xc3c86c1e5e12f03d, ; 938: WindowsBase => 169
	i64 u0xc421b61fd853169d, ; 939: lib_System.Net.WebSockets.Client.dll.so => 82
	i64 u0xc4261d083d5c9802, ; 940: Modules.Users.DTO => 401
	i64 u0xc463e077917aa21d, ; 941: System.Runtime.Serialization.Json => 115
	i64 u0xc46e8e6623ac5e82, ; 942: StackExchange.Redis.dll => 263
	i64 u0xc472ce300460ccb6, ; 943: Microsoft.EntityFrameworkCore.dll => 215
	i64 u0xc4d3858ed4d08512, ; 944: Xamarin.AndroidX.Lifecycle.ViewModelSavedState.dll => 318
	i64 u0xc4d69851fe06342f, ; 945: lib_Microsoft.Extensions.Caching.Memory.dll.so => 219
	i64 u0xc50fded0ded1418c, ; 946: lib_System.ComponentModel.TypeConverter.dll.so => 17
	i64 u0xc519125d6bc8fb11, ; 947: lib_System.Net.Requests.dll.so => 74
	i64 u0xc5293b19e4dc230e, ; 948: Xamarin.AndroidX.Navigation.Fragment => 323
	i64 u0xc5325b2fcb37446f, ; 949: lib_System.Private.Xml.dll.so => 91
	i64 u0xc535cb9a21385d9b, ; 950: lib_Xamarin.Android.Glide.DiskLruCache.dll.so => 271
	i64 u0xc5a0f4b95a699af7, ; 951: lib_System.Private.Uri.dll.so => 89
	i64 u0xc5cdcd5b6277579e, ; 952: lib_System.Security.Cryptography.Algorithms.dll.so => 122
	i64 u0xc5ec286825cb0bf4, ; 953: Xamarin.AndroidX.Tracing.Tracing => 337
	i64 u0xc6706bc8aa7fe265, ; 954: Xamarin.AndroidX.Annotation.Jvm => 277
	i64 u0xc74d70d4aa96cef3, ; 955: Xamarin.AndroidX.Navigation.Runtime.Android => 325
	i64 u0xc7c01e7d7c93a110, ; 956: System.Text.Encoding.Extensions.dll => 137
	i64 u0xc7ce851898a4548e, ; 957: lib_System.Web.HttpUtility.dll.so => 156
	i64 u0xc809d4089d2556b2, ; 958: System.Runtime.InteropServices.JavaScript.dll => 108
	i64 u0xc858a28d9ee5a6c5, ; 959: lib_System.Collections.Specialized.dll.so => 11
	i64 u0xc8ac7c6bf1c2ec51, ; 960: System.Reflection.DispatchProxy.dll => 92
	i64 u0xc99ccc413e3ce0d4, ; 961: lib_Microsoft.AspNetCore.Identity.EntityFrameworkCore.dll.so => 206
	i64 u0xc9c62c8f354ac568, ; 962: lib_System.Diagnostics.TextWriterTraceListener.dll.so => 31
	i64 u0xc9e54b32fc19baf3, ; 963: lib_CommunityToolkit.Maui.dll.so => 179
	i64 u0xca32340d8d54dcd5, ; 964: Microsoft.Extensions.Caching.Memory.dll => 219
	i64 u0xca3a723e7342c5b6, ; 965: lib-tr-Microsoft.Maui.Controls.resources.dll.so => 390
	i64 u0xca5801070d9fccfb, ; 966: System.Text.Encoding => 138
	i64 u0xcab3493c70141c2d, ; 967: pl/Microsoft.Maui.Controls.resources => 382
	i64 u0xcacfddc9f7c6de76, ; 968: ro/Microsoft.Maui.Controls.resources.dll => 385
	i64 u0xcadbc92899a777f0, ; 969: Xamarin.AndroidX.Startup.StartupRuntime => 335
	i64 u0xcb45618372c47127, ; 970: Microsoft.EntityFrameworkCore.Relational => 217
	i64 u0xcb6f731cbdfa3dd8, ; 971: Npgsql.EntityFrameworkCore.PostgreSQL => 254
	i64 u0xcba1cb79f45292b5, ; 972: Xamarin.Android.Glide.GifDecoder.dll => 272
	i64 u0xcbb5f80c7293e696, ; 973: lib_System.Globalization.Calendars.dll.so => 40
	i64 u0xcbd4fdd9cef4a294, ; 974: lib__Microsoft.Android.Resource.Designer.dll.so => 403
	i64 u0xcc15da1e07bbd994, ; 975: Xamarin.AndroidX.SlidingPaneLayout => 334
	i64 u0xcc2876b32ef2794c, ; 976: lib_System.Text.RegularExpressions.dll.so => 141
	i64 u0xcc5c3bb714c4561e, ; 977: Xamarin.KotlinX.Coroutines.Core.Jvm.dll => 358
	i64 u0xcc76886e09b88260, ; 978: Xamarin.KotlinX.Serialization.Core.Jvm.dll => 360
	i64 u0xcc9fa2923aa1c9ef, ; 979: System.Diagnostics.Contracts.dll => 25
	i64 u0xccae9bb73e2326bd, ; 980: lib_System.IO.Hashing.dll.so => 176
	i64 u0xccf25c4b634ccd3a, ; 981: zh-Hans/Microsoft.Maui.Controls.resources.dll => 394
	i64 u0xcd10a42808629144, ; 982: System.Net.Requests => 74
	i64 u0xcdca1b920e9f53ba, ; 983: Xamarin.AndroidX.Interpolator => 304
	i64 u0xcdd0c48b6937b21c, ; 984: Xamarin.AndroidX.SwipeRefreshLayout => 336
	i64 u0xcf23d8093f3ceadf, ; 985: System.Diagnostics.DiagnosticSource.dll => 27
	i64 u0xcf5ff6b6b2c4c382, ; 986: System.Net.Mail.dll => 68
	i64 u0xcf8fc898f98b0d34, ; 987: System.Private.Xml.Linq => 90
	i64 u0xcfcfd50998ac9247, ; 988: lib_FluentValidation.dll.so => 183
	i64 u0xd04b5f59ed596e31, ; 989: System.Reflection.Metadata.dll => 97
	i64 u0xd063299fcfc0c93f, ; 990: lib_System.Runtime.Serialization.Json.dll.so => 115
	i64 u0xd0de8a113e976700, ; 991: System.Diagnostics.TextWriterTraceListener => 31
	i64 u0xd0fc33d5ae5d4cb8, ; 992: System.Runtime.Extensions => 106
	i64 u0xd1194e1d8a8de83c, ; 993: lib_Xamarin.AndroidX.Lifecycle.Common.Jvm.dll.so => 306
	i64 u0xd12beacdfc14f696, ; 994: System.Dynamic.Runtime => 37
	i64 u0xd16fd7fb9bbcd43e, ; 995: Microsoft.Extensions.Diagnostics.Abstractions => 228
	i64 u0xd198e7ce1b6a8344, ; 996: System.Net.Quic.dll => 73
	i64 u0xd21d3815977b72a6, ; 997: lib_WorkoutLogg.dll.so => 0
	i64 u0xd3144156a3727ebe, ; 998: Xamarin.Google.Guava.ListenableFuture => 352
	i64 u0xd333d0af9e423810, ; 999: System.Runtime.InteropServices => 110
	i64 u0xd33a415cb4278969, ; 1000: System.Security.Cryptography.Encoding.dll => 125
	i64 u0xd3426d966bb704f5, ; 1001: Xamarin.AndroidX.AppCompat.AppCompatResources.dll => 279
	i64 u0xd3651b6fc3125825, ; 1002: System.Private.Uri.dll => 89
	i64 u0xd373685349b1fe8b, ; 1003: Microsoft.Extensions.Logging.dll => 234
	i64 u0xd3801faafafb7698, ; 1004: System.Private.DataContractSerialization.dll => 88
	i64 u0xd3e4c8d6a2d5d470, ; 1005: it/Microsoft.Maui.Controls.resources => 376
	i64 u0xd3edcc1f25459a50, ; 1006: System.Reflection.Emit => 95
	i64 u0xd42655883bb8c19f, ; 1007: Microsoft.EntityFrameworkCore.Abstractions.dll => 216
	i64 u0xd4645626dffec99d, ; 1008: lib_Microsoft.Extensions.DependencyInjection.Abstractions.dll.so => 225
	i64 u0xd4fa0abb79079ea9, ; 1009: System.Security.Principal.dll => 131
	i64 u0xd5507e11a2b2839f, ; 1010: Xamarin.AndroidX.Lifecycle.ViewModelSavedState => 318
	i64 u0xd5d04bef8478ea19, ; 1011: Xamarin.AndroidX.Tracing.Tracing.dll => 337
	i64 u0xd60815f26a12e140, ; 1012: Microsoft.Extensions.Logging.Debug.dll => 236
	i64 u0xd6694f8359737e4e, ; 1013: Xamarin.AndroidX.SavedState => 330
	i64 u0xd6949e129339eae5, ; 1014: lib_Xamarin.AndroidX.Core.Core.Ktx.dll.so => 292
	i64 u0xd6d21782156bc35b, ; 1015: Xamarin.AndroidX.SwipeRefreshLayout.dll => 336
	i64 u0xd6de019f6af72435, ; 1016: Xamarin.AndroidX.ConstraintLayout.Core.dll => 289
	i64 u0xd70956d1e6deefb9, ; 1017: Jsr305Binding => 349
	i64 u0xd72329819cbbbc44, ; 1018: lib_Microsoft.Extensions.Configuration.Abstractions.dll.so => 222
	i64 u0xd72c760af136e863, ; 1019: System.Xml.XmlSerializer.dll => 166
	i64 u0xd753f071e44c2a03, ; 1020: lib_System.Security.SecureString.dll.so => 132
	i64 u0xd7b3764ada9d341d, ; 1021: lib_Microsoft.Extensions.Logging.Abstractions.dll.so => 235
	i64 u0xd7f0088bc5ad71f2, ; 1022: Xamarin.AndroidX.VersionedParcelable => 342
	i64 u0xd824ef6ab33f8f7a, ; 1023: Xamarin.AndroidX.Window.WindowCore.dll => 346
	i64 u0xd8fb25e28ae30a12, ; 1024: Xamarin.AndroidX.ProfileInstaller.ProfileInstaller.dll => 327
	i64 u0xda1dfa4c534a9251, ; 1025: Microsoft.Extensions.DependencyInjection => 224
	i64 u0xdaaf1c9d5686de91, ; 1026: AKSoftware.Localization.MultiLanguages => 178
	i64 u0xdad05a11827959a3, ; 1027: System.Collections.NonGeneric.dll => 10
	i64 u0xdaefdfe71aa53cf9, ; 1028: System.IO.FileSystem.Primitives => 49
	i64 u0xdb5383ab5865c007, ; 1029: lib-vi-Microsoft.Maui.Controls.resources.dll.so => 392
	i64 u0xdb58816721c02a59, ; 1030: lib_System.Reflection.Emit.ILGeneration.dll.so => 93
	i64 u0xdb9f2880a64da6d6, ; 1031: Microsoft.Extensions.Identity.Stores.dll => 233
	i64 u0xdbeda89f832aa805, ; 1032: vi/Microsoft.Maui.Controls.resources.dll => 392
	i64 u0xdbf2a779fbc3ac31, ; 1033: System.Transactions.Local.dll => 153
	i64 u0xdbf9607a441b4505, ; 1034: System.Linq => 63
	i64 u0xdbfc90157a0de9b0, ; 1035: lib_System.Text.Encoding.dll.so => 138
	i64 u0xdc75032002d1a212, ; 1036: lib_System.Transactions.Local.dll.so => 153
	i64 u0xdca8be7403f92d4f, ; 1037: lib_System.Linq.Queryable.dll.so => 62
	i64 u0xdce2c53525640bf3, ; 1038: Microsoft.Extensions.Logging => 234
	i64 u0xdd2b722d78ef5f43, ; 1039: System.Runtime.dll => 119
	i64 u0xdd67031857c72f96, ; 1040: lib_System.Text.Encodings.Web.dll.so => 139
	i64 u0xdd70765ad6162057, ; 1041: Xamarin.JSpecify => 354
	i64 u0xdd92e229ad292030, ; 1042: System.Numerics.dll => 86
	i64 u0xdde30e6b77aa6f6c, ; 1043: lib-zh-Hans-Microsoft.Maui.Controls.resources.dll.so => 394
	i64 u0xde110ae80fa7c2e2, ; 1044: System.Xml.XDocument.dll => 162
	i64 u0xde37b315aaa4d425, ; 1045: Confluent.Kafka.dll => 182
	i64 u0xde4726fcdf63a198, ; 1046: Xamarin.AndroidX.Transition => 339
	i64 u0xde572c2b2fb32f93, ; 1047: lib_System.Threading.Tasks.Extensions.dll.so => 146
	i64 u0xde8769ebda7d8647, ; 1048: hr/Microsoft.Maui.Controls.resources.dll => 373
	i64 u0xdee075f3477ef6be, ; 1049: Xamarin.AndroidX.ExifInterface.dll => 301
	i64 u0xdf4b773de8fb1540, ; 1050: System.Net.dll => 84
	i64 u0xdf9c7682560a9629, ; 1051: System.Net.ServerSentEvents => 76
	i64 u0xdfa254ebb4346068, ; 1052: System.Net.Ping => 71
	i64 u0xdfa4850418b6c99a, ; 1053: Microsoft.AspNetCore.Hosting.Abstractions => 199
	i64 u0xe0142572c095a480, ; 1054: Xamarin.AndroidX.AppCompat.dll => 278
	i64 u0xe020c74e3723dc6f, ; 1055: Syncfusion.Maui.Toolkit.dll => 264
	i64 u0xe021eaa401792a05, ; 1056: System.Text.Encoding.dll => 138
	i64 u0xe02f89350ec78051, ; 1057: Xamarin.AndroidX.CoordinatorLayout.dll => 290
	i64 u0xe0496b9d65ef5474, ; 1058: Xamarin.Android.Glide.DiskLruCache.dll => 271
	i64 u0xe0be470debe77c12, ; 1059: Microsoft.AspNetCore.Cryptography.Internal.dll => 195
	i64 u0xe10b760bb1462e7a, ; 1060: lib_System.Security.Cryptography.Primitives.dll.so => 127
	i64 u0xe192a588d4410686, ; 1061: lib_System.IO.Pipelines.dll.so => 54
	i64 u0xe1a08bd3fa539e0d, ; 1062: System.Runtime.Loader => 112
	i64 u0xe1a77eb8831f7741, ; 1063: System.Security.SecureString.dll => 132
	i64 u0xe1b52f9f816c70ef, ; 1064: System.Private.Xml.Linq.dll => 90
	i64 u0xe1e199c8ab02e356, ; 1065: System.Data.DataSetExtensions.dll => 23
	i64 u0xe1ecfdb7fff86067, ; 1066: System.Net.Security.dll => 75
	i64 u0xe2252a80fe853de4, ; 1067: lib_System.Security.Principal.dll.so => 131
	i64 u0xe22fa4c9c645db62, ; 1068: System.Diagnostics.TextWriterTraceListener.dll => 31
	i64 u0xe24095a7afddaab3, ; 1069: lib_Microsoft.Extensions.Hosting.Abstractions.dll.so => 230
	i64 u0xe2420585aeceb728, ; 1070: System.Net.Requests.dll => 74
	i64 u0xe26692647e6bcb62, ; 1071: Xamarin.AndroidX.Lifecycle.Runtime.Ktx => 313
	i64 u0xe29b73bc11392966, ; 1072: lib-id-Microsoft.Maui.Controls.resources.dll.so => 375
	i64 u0xe2ad448dee50fbdf, ; 1073: System.Xml.Serialization => 161
	i64 u0xe2d920f978f5d85c, ; 1074: System.Data.DataSetExtensions => 23
	i64 u0xe2e426c7714fa0bc, ; 1075: Microsoft.Win32.Primitives.dll => 4
	i64 u0xe332bacb3eb4a806, ; 1076: Mono.Android.Export.dll => 173
	i64 u0xe3811d68d4fe8463, ; 1077: pt-BR/Microsoft.Maui.Controls.resources.dll => 383
	i64 u0xe38e8ac420aab422, ; 1078: Microsoft.AspNetCore.Mvc.Core => 208
	i64 u0xe3a586956771a0ed, ; 1079: lib_SQLite-net.dll.so => 258
	i64 u0xe3b7cbae5ad66c75, ; 1080: lib_System.Security.Cryptography.Encoding.dll.so => 125
	i64 u0xe4292b48f3224d5b, ; 1081: lib_Xamarin.AndroidX.Core.ViewTree.dll.so => 293
	i64 u0xe494f7ced4ecd10a, ; 1082: hu/Microsoft.Maui.Controls.resources.dll => 374
	i64 u0xe4a9b1e40d1e8917, ; 1083: lib-fi-Microsoft.Maui.Controls.resources.dll.so => 369
	i64 u0xe4f74a0b5bf9703f, ; 1084: System.Runtime.Serialization.Primitives => 116
	i64 u0xe525d740098b0a3c, ; 1085: lib_Moduels.Workouts.DTO.dll.so => 396
	i64 u0xe5434e8a119ceb69, ; 1086: lib_Mono.Android.dll.so => 175
	i64 u0xe55703b9ce5c038a, ; 1087: System.Diagnostics.Tools => 32
	i64 u0xe57013c8afc270b5, ; 1088: Microsoft.VisualBasic => 3
	i64 u0xe62913cc36bc07ec, ; 1089: System.Xml.dll => 167
	i64 u0xe66e263beb16318f, ; 1090: Microsoft.Extensions.WebEncoders => 241
	i64 u0xe7bea09c4900a191, ; 1091: Xamarin.AndroidX.VectorDrawable.dll => 340
	i64 u0xe7e03cc18dcdeb49, ; 1092: lib_System.Diagnostics.StackTrace.dll.so => 30
	i64 u0xe7e147ff99a7a380, ; 1093: lib_System.Configuration.dll.so => 19
	i64 u0xe8397cf3948e7cb7, ; 1094: lib_Microsoft.Extensions.Options.ConfigurationExtensions.dll.so => 239
	i64 u0xe86b0df4ba9e5db8, ; 1095: lib_Xamarin.AndroidX.Lifecycle.Runtime.Android.dll.so => 312
	i64 u0xe896622fe0902957, ; 1096: System.Reflection.Emit.dll => 95
	i64 u0xe89a2a9ef110899b, ; 1097: System.Drawing.dll => 36
	i64 u0xe8c35a466559994c, ; 1098: lib_Microsoft.Extensions.Identity.Stores.dll.so => 233
	i64 u0xe8c5f8c100b5934b, ; 1099: Microsoft.Win32.Registry => 5
	i64 u0xe98163eb702ae5c5, ; 1100: Xamarin.AndroidX.Arch.Core.Runtime => 281
	i64 u0xe98b0e4b4d44e931, ; 1101: lib_Grpc.Net.Client.dll.so => 187
	i64 u0xe994f23ba4c143e5, ; 1102: Xamarin.KotlinX.Coroutines.Android => 356
	i64 u0xe9b9c8c0458fd92a, ; 1103: System.Windows => 158
	i64 u0xe9d166d87a7f2bdb, ; 1104: lib_Xamarin.AndroidX.Startup.StartupRuntime.dll.so => 335
	i64 u0xea008206567504c4, ; 1105: Syncfusion.Maui.Toolkit => 264
	i64 u0xea5a4efc2ad81d1b, ; 1106: Xamarin.Google.ErrorProne.Annotations => 351
	i64 u0xeb2313fe9d65b785, ; 1107: Xamarin.AndroidX.ConstraintLayout.dll => 288
	i64 u0xec8abb68d340aac6, ; 1108: Microsoft.AspNetCore.Authorization => 193
	i64 u0xed19c616b3fcb7eb, ; 1109: Xamarin.AndroidX.VersionedParcelable.dll => 342
	i64 u0xedc4817167106c23, ; 1110: System.Net.Sockets.dll => 78
	i64 u0xedc632067fb20ff3, ; 1111: System.Memory.dll => 64
	i64 u0xedc8e4ca71a02a8b, ; 1112: Xamarin.AndroidX.Navigation.Runtime.dll => 324
	i64 u0xee81f5b3f1c4f83b, ; 1113: System.Threading.ThreadPool => 150
	i64 u0xeeb7ebb80150501b, ; 1114: lib_Xamarin.AndroidX.Collection.Jvm.dll.so => 285
	i64 u0xeefc635595ef57f0, ; 1115: System.Security.Cryptography.Cng => 123
	i64 u0xef03b1b5a04e9709, ; 1116: System.Text.Encoding.CodePages.dll => 136
	i64 u0xef5bcbe61622ee5f, ; 1117: Xamarin.AndroidX.Tracing.Tracing.Android.dll => 338
	i64 u0xef602c523fe2e87a, ; 1118: lib_Xamarin.Google.Guava.ListenableFuture.dll.so => 352
	i64 u0xef72742e1bcca27a, ; 1119: Microsoft.Maui.Essentials.dll => 249
	i64 u0xefd1e0c4e5c9b371, ; 1120: System.Resources.ResourceManager.dll => 102
	i64 u0xefe8f8d5ed3c72ea, ; 1121: System.Formats.Tar.dll => 39
	i64 u0xefec0b7fdc57ec42, ; 1122: Xamarin.AndroidX.Activity => 273
	i64 u0xeff59cbde4363ec3, ; 1123: System.Threading.AccessControl.dll => 142
	i64 u0xf00c29406ea45e19, ; 1124: es/Microsoft.Maui.Controls.resources.dll => 368
	i64 u0xf020834425394c93, ; 1125: Microsoft.AspNetCore.ResponseCaching.Abstractions.dll => 209
	i64 u0xf09e47b6ae914f6e, ; 1126: System.Net.NameResolution => 69
	i64 u0xf0ac2b489fed2e35, ; 1127: lib_System.Diagnostics.Debug.dll.so => 26
	i64 u0xf0bb49dadd3a1fe1, ; 1128: lib_System.Net.ServicePoint.dll.so => 77
	i64 u0xf0c16dff90fbf5d6, ; 1129: Xamarin.AndroidX.Window.WindowCore.Jvm => 347
	i64 u0xf0de2537ee19c6ca, ; 1130: lib_System.Net.WebHeaderCollection.dll.so => 80
	i64 u0xf1138779fa181c68, ; 1131: lib_Xamarin.AndroidX.Lifecycle.Runtime.dll.so => 311
	i64 u0xf11b621fc87b983f, ; 1132: Microsoft.Maui.Controls.Xaml.dll => 247
	i64 u0xf161bf2d1e9eaff4, ; 1133: lib_Microsoft.AspNetCore.DataProtection.dll.so => 197
	i64 u0xf161f4f3c3b7e62c, ; 1134: System.Data => 24
	i64 u0xf16eb650d5a464bc, ; 1135: System.ValueTuple => 155
	i64 u0xf1c4b4005493d871, ; 1136: System.Formats.Asn1.dll => 38
	i64 u0xf22514cfad2d598b, ; 1137: lib_Xamarin.AndroidX.Lifecycle.ViewModelSavedState.Android.dll.so => 319
	i64 u0xf238bd79489d3a96, ; 1138: lib-nl-Microsoft.Maui.Controls.resources.dll.so => 381
	i64 u0xf2feea356ba760af, ; 1139: Xamarin.AndroidX.Arch.Core.Runtime.dll => 281
	i64 u0xf300e085f8acd238, ; 1140: lib_System.ServiceProcess.dll.so => 135
	i64 u0xf34e52b26e7e059d, ; 1141: System.Runtime.CompilerServices.VisualC.dll => 105
	i64 u0xf37221fda4ef8830, ; 1142: lib_Xamarin.Google.Android.Material.dll.so => 348
	i64 u0xf3ad9b8fb3eefd12, ; 1143: lib_System.IO.UnmanagedMemoryStream.dll.so => 57
	i64 u0xf3ddfe05336abf29, ; 1144: System => 168
	i64 u0xf408654b2a135055, ; 1145: System.Reflection.Emit.ILGeneration.dll => 93
	i64 u0xf4103170a1de5bd0, ; 1146: System.Linq.Queryable.dll => 62
	i64 u0xf4113226370f57aa, ; 1147: lib_YamlDotNet.dll.so => 361
	i64 u0xf42ad2f4323b64d3, ; 1148: Microsoft.Net.Http.Headers.dll => 251
	i64 u0xf42d20c23173d77c, ; 1149: lib_System.ServiceModel.Web.dll.so => 134
	i64 u0xf4c1dd70a5496a17, ; 1150: System.IO.Compression => 46
	i64 u0xf4ecf4b9afc64781, ; 1151: System.ServiceProcess.dll => 135
	i64 u0xf4eeeaa566e9b970, ; 1152: lib_Xamarin.AndroidX.CustomView.PoolingContainer.dll.so => 296
	i64 u0xf518f63ead11fcd1, ; 1153: System.Threading.Tasks => 148
	i64 u0xf5fc7602fe27b333, ; 1154: System.Net.WebHeaderCollection => 80
	i64 u0xf6077741019d7428, ; 1155: Xamarin.AndroidX.CoordinatorLayout => 290
	i64 u0xf61ade9836ad4692, ; 1156: Microsoft.IdentityModel.Tokens.dll => 245
	i64 u0xf6742cbf457c450b, ; 1157: Xamarin.AndroidX.Lifecycle.Runtime.Android.dll => 312
	i64 u0xf6c0e7d55a7a4e4f, ; 1158: Microsoft.IdentityModel.JsonWebTokens => 243
	i64 u0xf6e8de2aebcbb422, ; 1159: lib_Xamarin.AndroidX.Window.WindowCore.Jvm.dll.so => 347
	i64 u0xf6f893f692f8cb43, ; 1160: Microsoft.Extensions.Options.ConfigurationExtensions.dll => 239
	i64 u0xf70c0a7bf8ccf5af, ; 1161: System.Web => 157
	i64 u0xf77b20923f07c667, ; 1162: de/Microsoft.Maui.Controls.resources.dll => 366
	i64 u0xf79cbf52994c8548, ; 1163: Npgsql => 253
	i64 u0xf7be38c7938ad857, ; 1164: Microsoft.AspNetCore.Cryptography.KeyDerivation => 196
	i64 u0xf7e2cac4c45067b3, ; 1165: lib_System.Numerics.Vectors.dll.so => 85
	i64 u0xf7e74930e0e3d214, ; 1166: zh-HK/Microsoft.Maui.Controls.resources.dll => 393
	i64 u0xf7fa0bf77fe677cc, ; 1167: Newtonsoft.Json.dll => 252
	i64 u0xf84773b5c81e3cef, ; 1168: lib-uk-Microsoft.Maui.Controls.resources.dll.so => 391
	i64 u0xf8aac5ea82de1348, ; 1169: System.Linq.Queryable => 62
	i64 u0xf8b77539b362d3ba, ; 1170: lib_System.Reflection.Primitives.dll.so => 98
	i64 u0xf8e045dc345b2ea3, ; 1171: lib_Xamarin.AndroidX.RecyclerView.dll.so => 328
	i64 u0xf915dc29808193a1, ; 1172: System.Web.HttpUtility.dll => 156
	i64 u0xf96c777a2a0686f4, ; 1173: hi/Microsoft.Maui.Controls.resources.dll => 372
	i64 u0xf9be54c8bcf8ff3b, ; 1174: System.Security.AccessControl.dll => 120
	i64 u0xf9eec5bb3a6aedc6, ; 1175: Microsoft.Extensions.Options => 238
	i64 u0xfa0e82300e67f913, ; 1176: lib_System.AppContext.dll.so => 6
	i64 u0xfa2fdb27e8a2c8e8, ; 1177: System.ComponentModel.EventBasedAsync => 15
	i64 u0xfa3f278f288b0e84, ; 1178: lib_System.Net.Security.dll.so => 75
	i64 u0xfa504dfa0f097d72, ; 1179: Microsoft.Extensions.FileProviders.Abstractions.dll => 229
	i64 u0xfa5ed7226d978949, ; 1180: lib-ar-Microsoft.Maui.Controls.resources.dll.so => 362
	i64 u0xfa645d91e9fc4cba, ; 1181: System.Threading.Thread => 149
	i64 u0xfad4d2c770e827f9, ; 1182: lib_System.IO.IsolatedStorage.dll.so => 52
	i64 u0xfb022853d73b7fa5, ; 1183: lib_SQLitePCLRaw.batteries_v2.dll.so => 259
	i64 u0xfb06dd2338e6f7c4, ; 1184: System.Net.Ping.dll => 71
	i64 u0xfb087abe5365e3b7, ; 1185: lib_System.Data.DataSetExtensions.dll.so => 23
	i64 u0xfb2f5086cd5f5de4, ; 1186: lib_StackExchange.Redis.dll.so => 263
	i64 u0xfb846e949baff5ea, ; 1187: System.Xml.Serialization.dll => 161
	i64 u0xfbad3e4ce4b98145, ; 1188: System.Security.Cryptography.X509Certificates => 128
	i64 u0xfbd71978549ea473, ; 1189: Microsoft.AspNetCore.Http.Features.dll => 204
	i64 u0xfbf0a31c9fc34bc4, ; 1190: lib_System.Net.Http.dll.so => 66
	i64 u0xfc434411e14afaaf, ; 1191: Moduels.Workouts.DTO => 396
	i64 u0xfc6b7527cc280b3f, ; 1192: lib_System.Runtime.Serialization.Formatters.dll.so => 114
	i64 u0xfc719aec26adf9d9, ; 1193: Xamarin.AndroidX.Navigation.Fragment.dll => 323
	i64 u0xfc82690c2fe2735c, ; 1194: Xamarin.AndroidX.Lifecycle.Process.dll => 310
	i64 u0xfc93fc307d279893, ; 1195: System.IO.Pipes.AccessControl.dll => 55
	i64 u0xfcd302092ada6328, ; 1196: System.IO.MemoryMappedFiles.dll => 53
	i64 u0xfd22f00870e40ae0, ; 1197: lib_Xamarin.AndroidX.DrawerLayout.dll.so => 297
	i64 u0xfd49b3c1a76e2748, ; 1198: System.Runtime.InteropServices.RuntimeInformation => 109
	i64 u0xfd536c702f64dc47, ; 1199: System.Text.Encoding.Extensions => 137
	i64 u0xfd583f7657b6a1cb, ; 1200: Xamarin.AndroidX.Fragment => 302
	i64 u0xfd8dd91a2c26bd5d, ; 1201: Xamarin.AndroidX.Lifecycle.Runtime => 311
	i64 u0xfda36abccf05cf5c, ; 1202: System.Net.WebSockets.Client => 82
	i64 u0xfdbe4710aa9beeff, ; 1203: CommunityToolkit.Maui => 179
	i64 u0xfddbe9695626a7f5, ; 1204: Xamarin.AndroidX.Lifecycle.Common => 305
	i64 u0xfeae9952cf03b8cb, ; 1205: tr/Microsoft.Maui.Controls.resources => 390
	i64 u0xfebe1950717515f9, ; 1206: Xamarin.AndroidX.Lifecycle.LiveData.Core.Ktx.dll => 309
	i64 u0xff1a4e86e72b0140, ; 1207: Microsoft.AspNetCore.Authentication.Abstractions.dll => 190
	i64 u0xff270a55858bac8d, ; 1208: System.Security.Principal => 131
	i64 u0xff9b54613e0d2cc8, ; 1209: System.Net.Http.Json => 65
	i64 u0xffd5b3e75321a00b, ; 1210: Microsoft.AspNetCore.DataProtection.Abstractions => 198
	i64 u0xffdb7a971be4ec73 ; 1211: System.ValueTuple.dll => 155
], align 8

@assembly_image_cache_indices = dso_local local_unnamed_addr constant [1212 x i32] [
	i32 42, i32 357, i32 336, i32 256, i32 13, i32 194, i32 187, i32 324,
	i32 180, i32 239, i32 267, i32 107, i32 219, i32 174, i32 48, i32 278,
	i32 7, i32 262, i32 198, i32 88, i32 386, i32 364, i32 392, i32 242,
	i32 298, i32 72, i32 328, i32 213, i32 12, i32 248, i32 104, i32 200,
	i32 213, i32 393, i32 159, i32 19, i32 303, i32 285, i32 164, i32 300,
	i32 241, i32 340, i32 170, i32 386, i32 10, i32 236, i32 341, i32 98,
	i32 296, i32 297, i32 13, i32 238, i32 10, i32 129, i32 198, i32 97,
	i32 218, i32 268, i32 143, i32 39, i32 387, i32 360, i32 196, i32 343,
	i32 383, i32 175, i32 272, i32 5, i32 249, i32 68, i32 333, i32 132,
	i32 232, i32 186, i32 332, i32 299, i32 69, i32 286, i32 67, i32 199,
	i32 57, i32 295, i32 52, i32 43, i32 237, i32 127, i32 68, i32 83,
	i32 313, i32 161, i32 94, i32 101, i32 328, i32 144, i32 154, i32 282,
	i32 370, i32 165, i32 172, i32 371, i32 225, i32 83, i32 354, i32 286,
	i32 4, i32 5, i32 51, i32 103, i32 226, i32 56, i32 122, i32 100,
	i32 171, i32 120, i32 357, i32 21, i32 254, i32 374, i32 139, i32 397,
	i32 99, i32 360, i32 79, i32 380, i32 257, i32 398, i32 335, i32 121,
	i32 200, i32 8, i32 168, i32 389, i32 71, i32 271, i32 314, i32 329,
	i32 203, i32 227, i32 174, i32 148, i32 40, i32 333, i32 47, i32 184,
	i32 30, i32 326, i32 378, i32 147, i32 238, i32 166, i32 232, i32 28,
	i32 86, i32 337, i32 208, i32 79, i32 43, i32 29, i32 42, i32 105,
	i32 119, i32 276, i32 45, i32 93, i32 389, i32 56, i32 151, i32 149,
	i32 215, i32 102, i32 49, i32 20, i32 291, i32 116, i32 269, i32 370,
	i32 350, i32 259, i32 355, i32 240, i32 96, i32 58, i32 266, i32 375,
	i32 373, i32 346, i32 83, i32 350, i32 172, i32 26, i32 72, i32 327,
	i32 256, i32 212, i32 228, i32 301, i32 322, i32 391, i32 70, i32 33,
	i32 369, i32 14, i32 141, i32 266, i32 38, i32 395, i32 287, i32 212,
	i32 382, i32 402, i32 136, i32 94, i32 90, i32 152, i32 347, i32 388,
	i32 24, i32 140, i32 57, i32 142, i32 396, i32 51, i32 367, i32 263,
	i32 29, i32 160, i32 34, i32 167, i32 218, i32 231, i32 183, i32 302,
	i32 242, i32 52, i32 178, i32 403, i32 345, i32 92, i32 283, i32 35,
	i32 370, i32 160, i32 9, i32 368, i32 78, i32 59, i32 55, i32 248,
	i32 364, i32 246, i32 13, i32 344, i32 221, i32 280, i32 111, i32 189,
	i32 317, i32 253, i32 32, i32 106, i32 86, i32 94, i32 53, i32 98,
	i32 189, i32 353, i32 58, i32 195, i32 9, i32 104, i32 295, i32 69,
	i32 194, i32 343, i32 363, i32 252, i32 229, i32 127, i32 329, i32 118,
	i32 137, i32 331, i32 245, i32 128, i32 108, i32 355, i32 133, i32 203,
	i32 282, i32 352, i32 150, i32 159, i32 303, i32 291, i32 259, i32 184,
	i32 298, i32 329, i32 99, i32 201, i32 24, i32 334, i32 146, i32 230,
	i32 321, i32 3, i32 220, i32 170, i32 279, i32 102, i32 164, i32 101,
	i32 293, i32 191, i32 25, i32 205, i32 95, i32 171, i32 175, i32 274,
	i32 3, i32 382, i32 338, i32 300, i32 1, i32 116, i32 355, i32 192,
	i32 216, i32 303, i32 310, i32 266, i32 33, i32 6, i32 226, i32 268,
	i32 386, i32 159, i32 188, i32 384, i32 53, i32 241, i32 87, i32 251,
	i32 342, i32 326, i32 44, i32 309, i32 106, i32 47, i32 209, i32 140,
	i32 207, i32 319, i32 65, i32 217, i32 320, i32 70, i32 82, i32 60,
	i32 91, i32 157, i32 280, i32 135, i32 112, i32 59, i32 376, i32 320,
	i32 327, i32 174, i32 233, i32 136, i32 143, i32 40, i32 363, i32 261,
	i32 223, i32 208, i32 245, i32 246, i32 188, i32 61, i32 181, i32 223,
	i32 316, i32 81, i32 25, i32 36, i32 190, i32 101, i32 313, i32 72,
	i32 202, i32 188, i32 22, i32 291, i32 250, i32 195, i32 387, i32 123,
	i32 70, i32 109, i32 393, i32 214, i32 121, i32 119, i32 305, i32 197,
	i32 322, i32 306, i32 11, i32 2, i32 126, i32 117, i32 145, i32 41,
	i32 89, i32 275, i32 260, i32 177, i32 230, i32 27, i32 151, i32 223,
	i32 377, i32 224, i32 251, i32 351, i32 274, i32 1, i32 200, i32 276,
	i32 44, i32 290, i32 152, i32 18, i32 0, i32 88, i32 365, i32 41,
	i32 309, i32 284, i32 207, i32 314, i32 96, i32 234, i32 28, i32 0,
	i32 41, i32 210, i32 80, i32 299, i32 287, i32 147, i32 110, i32 285,
	i32 11, i32 107, i32 139, i32 16, i32 124, i32 67, i32 160, i32 22,
	i32 261, i32 367, i32 359, i32 104, i32 237, i32 224, i32 358, i32 64,
	i32 58, i32 247, i32 366, i32 112, i32 177, i32 325, i32 356, i32 9,
	i32 348, i32 122, i32 100, i32 107, i32 76, i32 402, i32 317, i32 179,
	i32 256, i32 246, i32 113, i32 277, i32 49, i32 59, i32 20, i32 316,
	i32 294, i32 73, i32 289, i32 158, i32 39, i32 365, i32 35, i32 38,
	i32 371, i32 261, i32 110, i32 380, i32 21, i32 182, i32 361, i32 353,
	i32 315, i32 250, i32 15, i32 240, i32 81, i32 81, i32 294, i32 240,
	i32 400, i32 323, i32 332, i32 155, i32 21, i32 248, i32 364, i32 50,
	i32 51, i32 390, i32 380, i32 96, i32 270, i32 228, i32 376, i32 16,
	i32 267, i32 293, i32 125, i32 373, i32 163, i32 45, i32 351, i32 185,
	i32 118, i32 64, i32 169, i32 206, i32 227, i32 221, i32 14, i32 330,
	i32 113, i32 277, i32 61, i32 76, i32 123, i32 379, i32 2, i32 389,
	i32 302, i32 315, i32 231, i32 354, i32 315, i32 6, i32 284, i32 369,
	i32 298, i32 213, i32 243, i32 17, i32 387, i32 366, i32 79, i32 288,
	i32 322, i32 255, i32 180, i32 133, i32 353, i32 255, i32 379, i32 398,
	i32 85, i32 236, i32 12, i32 34, i32 121, i32 268, i32 359, i32 310,
	i32 176, i32 300, i32 87, i32 269, i32 196, i32 18, i32 204, i32 343,
	i32 222, i32 308, i32 194, i32 73, i32 97, i32 262, i32 168, i32 304,
	i32 84, i32 395, i32 237, i32 400, i32 278, i32 283, i32 157, i32 36,
	i32 154, i32 391, i32 258, i32 242, i32 394, i32 147, i32 56, i32 115,
	i32 217, i32 201, i32 284, i32 340, i32 339, i32 37, i32 395, i32 221,
	i32 117, i32 276, i32 14, i32 270, i32 216, i32 149, i32 202, i32 43,
	i32 187, i32 249, i32 274, i32 100, i32 358, i32 182, i32 171, i32 16,
	i32 331, i32 205, i32 48, i32 109, i32 203, i32 204, i32 99, i32 218,
	i32 320, i32 402, i32 27, i32 130, i32 29, i32 371, i32 192, i32 229,
	i32 332, i32 130, i32 44, i32 294, i32 299, i32 152, i32 8, i32 199,
	i32 186, i32 265, i32 338, i32 252, i32 321, i32 372, i32 385, i32 260,
	i32 384, i32 134, i32 383, i32 210, i32 42, i32 359, i32 260, i32 33,
	i32 211, i32 403, i32 46, i32 146, i32 316, i32 247, i32 307, i32 295,
	i32 140, i32 63, i32 134, i32 363, i32 48, i32 163, i32 281, i32 265,
	i32 307, i32 270, i32 361, i32 305, i32 379, i32 339, i32 400, i32 46,
	i32 167, i32 244, i32 176, i32 304, i32 244, i32 257, i32 368, i32 301,
	i32 375, i32 250, i32 18, i32 8, i32 185, i32 292, i32 398, i32 331,
	i32 126, i32 60, i32 144, i32 181, i32 324, i32 378, i32 311, i32 349,
	i32 253, i32 345, i32 153, i32 145, i32 357, i32 128, i32 356, i32 397,
	i32 163, i32 165, i32 296, i32 273, i32 222, i32 325, i32 381, i32 26,
	i32 321, i32 308, i32 401, i32 84, i32 220, i32 345, i32 129, i32 350,
	i32 103, i32 151, i32 183, i32 348, i32 346, i32 326, i32 254, i32 54,
	i32 165, i32 170, i32 133, i32 267, i32 37, i32 341, i32 378, i32 211,
	i32 227, i32 181, i32 22, i32 114, i32 92, i32 50, i32 61, i32 124,
	i32 85, i32 129, i32 214, i32 166, i32 349, i32 169, i32 330, i32 333,
	i32 297, i32 269, i32 312, i32 202, i32 4, i32 306, i32 374, i32 173,
	i32 2, i32 317, i32 118, i32 399, i32 243, i32 275, i32 19, i32 180,
	i32 235, i32 91, i32 66, i32 30, i32 225, i32 367, i32 289, i32 60,
	i32 207, i32 113, i32 308, i32 32, i32 184, i32 130, i32 162, i32 385,
	i32 191, i32 287, i32 143, i32 381, i32 156, i32 17, i32 286, i32 272,
	i32 77, i32 75, i32 15, i32 172, i32 87, i32 258, i32 265, i32 126,
	i32 307, i32 318, i32 288, i32 388, i32 314, i32 34, i32 232, i32 120,
	i32 141, i32 124, i32 108, i32 365, i32 341, i32 283, i32 178, i32 372,
	i32 362, i32 399, i32 54, i32 47, i32 28, i32 214, i32 192, i32 142,
	i32 212, i32 148, i32 235, i32 191, i32 211, i32 150, i32 35, i32 388,
	i32 397, i32 177, i32 231, i32 77, i32 164, i32 186, i32 1, i32 197,
	i32 319, i32 334, i32 384, i32 377, i32 162, i32 12, i32 158, i32 154,
	i32 201, i32 78, i32 105, i32 114, i32 399, i32 262, i32 280, i32 193,
	i32 66, i32 67, i32 344, i32 45, i32 282, i32 255, i32 111, i32 190,
	i32 209, i32 7, i32 279, i32 55, i32 275, i32 189, i32 65, i32 362,
	i32 292, i32 20, i32 111, i32 103, i32 63, i32 264, i32 145, i32 215,
	i32 193, i32 273, i32 206, i32 7, i32 244, i32 205, i32 377, i32 173,
	i32 50, i32 344, i32 117, i32 226, i32 144, i32 401, i32 210, i32 220,
	i32 185, i32 257, i32 169, i32 82, i32 401, i32 115, i32 263, i32 215,
	i32 318, i32 219, i32 17, i32 74, i32 323, i32 91, i32 271, i32 89,
	i32 122, i32 337, i32 277, i32 325, i32 137, i32 156, i32 108, i32 11,
	i32 92, i32 206, i32 31, i32 179, i32 219, i32 390, i32 138, i32 382,
	i32 385, i32 335, i32 217, i32 254, i32 272, i32 40, i32 403, i32 334,
	i32 141, i32 358, i32 360, i32 25, i32 176, i32 394, i32 74, i32 304,
	i32 336, i32 27, i32 68, i32 90, i32 183, i32 97, i32 115, i32 31,
	i32 106, i32 306, i32 37, i32 228, i32 73, i32 0, i32 352, i32 110,
	i32 125, i32 279, i32 89, i32 234, i32 88, i32 376, i32 95, i32 216,
	i32 225, i32 131, i32 318, i32 337, i32 236, i32 330, i32 292, i32 336,
	i32 289, i32 349, i32 222, i32 166, i32 132, i32 235, i32 342, i32 346,
	i32 327, i32 224, i32 178, i32 10, i32 49, i32 392, i32 93, i32 233,
	i32 392, i32 153, i32 63, i32 138, i32 153, i32 62, i32 234, i32 119,
	i32 139, i32 354, i32 86, i32 394, i32 162, i32 182, i32 339, i32 146,
	i32 373, i32 301, i32 84, i32 76, i32 71, i32 199, i32 278, i32 264,
	i32 138, i32 290, i32 271, i32 195, i32 127, i32 54, i32 112, i32 132,
	i32 90, i32 23, i32 75, i32 131, i32 31, i32 230, i32 74, i32 313,
	i32 375, i32 161, i32 23, i32 4, i32 173, i32 383, i32 208, i32 258,
	i32 125, i32 293, i32 374, i32 369, i32 116, i32 396, i32 175, i32 32,
	i32 3, i32 167, i32 241, i32 340, i32 30, i32 19, i32 239, i32 312,
	i32 95, i32 36, i32 233, i32 5, i32 281, i32 187, i32 356, i32 158,
	i32 335, i32 264, i32 351, i32 288, i32 193, i32 342, i32 78, i32 64,
	i32 324, i32 150, i32 285, i32 123, i32 136, i32 338, i32 352, i32 249,
	i32 102, i32 39, i32 273, i32 142, i32 368, i32 209, i32 69, i32 26,
	i32 77, i32 347, i32 80, i32 311, i32 247, i32 197, i32 24, i32 155,
	i32 38, i32 319, i32 381, i32 281, i32 135, i32 105, i32 348, i32 57,
	i32 168, i32 93, i32 62, i32 361, i32 251, i32 134, i32 46, i32 135,
	i32 296, i32 148, i32 80, i32 290, i32 245, i32 312, i32 243, i32 347,
	i32 239, i32 157, i32 366, i32 253, i32 196, i32 85, i32 393, i32 252,
	i32 391, i32 62, i32 98, i32 328, i32 156, i32 372, i32 120, i32 238,
	i32 6, i32 15, i32 75, i32 229, i32 362, i32 149, i32 52, i32 259,
	i32 71, i32 23, i32 263, i32 161, i32 128, i32 204, i32 66, i32 396,
	i32 114, i32 323, i32 310, i32 55, i32 53, i32 297, i32 109, i32 137,
	i32 302, i32 311, i32 82, i32 179, i32 305, i32 390, i32 309, i32 190,
	i32 131, i32 65, i32 198, i32 155
], align 4

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

; Function attributes: memory(write, argmem: none, inaccessiblemem: none) "min-legal-vector-width"="0" mustprogress "no-trapping-math"="true" nofree norecurse nosync nounwind "stack-protector-buffer-size"="8" uwtable willreturn
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
@.mm.0 = private unnamed_addr constant [40 x i8] c"get_function_pointer MUST be specified\0A\00", align 1

;MarshalMethodName
@.MarshalMethodName.0_name = private unnamed_addr constant [1 x i8] c"\00", align 1

; External functions

; Function attributes: "no-trapping-math"="true" noreturn nounwind "stack-protector-buffer-size"="8"
declare void @abort() local_unnamed_addr #2

; Function attributes: nofree nounwind
declare noundef i32 @puts(ptr noundef) local_unnamed_addr #1
attributes #0 = { memory(write, argmem: none, inaccessiblemem: none) "min-legal-vector-width"="0" mustprogress "no-trapping-math"="true" nofree norecurse nosync nounwind "stack-protector-buffer-size"="8" "target-cpu"="generic" "target-features"="+fix-cortex-a53-835769,+neon,+outline-atomics,+v8a" uwtable willreturn }
attributes #1 = { nofree nounwind }
attributes #2 = { "no-trapping-math"="true" noreturn nounwind "stack-protector-buffer-size"="8" "target-cpu"="generic" "target-features"="+fix-cortex-a53-835769,+neon,+outline-atomics,+v8a" }

; Metadata
!llvm.module.flags = !{!0, !1, !7, !8, !9, !10}
!0 = !{i32 1, !"wchar_size", i32 4}
!1 = !{i32 7, !"PIC Level", i32 2}
!llvm.ident = !{!2}
!2 = !{!".NET for Android remotes/origin/release/10.0.1xx @ 350a375fc202f0072ac4191624986d8c642b93fa"}
!3 = !{!4, !4, i64 0}
!4 = !{!"any pointer", !5, i64 0}
!5 = !{!"omnipotent char", !6, i64 0}
!6 = !{!"Simple C++ TBAA"}
!7 = !{i32 1, !"branch-target-enforcement", i32 0}
!8 = !{i32 1, !"sign-return-address", i32 0}
!9 = !{i32 1, !"sign-return-address-all", i32 0}
!10 = !{i32 1, !"sign-return-address-with-bkey", i32 0}
