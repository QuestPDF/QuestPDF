// AUTO-GENERATED on 02/23/2026 17:01:08
// QuestPDF TypeScript Bindings using koffi

import koffi from 'koffi';
import * as path from 'path';
import * as fs from 'fs';
import { fileURLToPath } from 'url';

// ============================================================================
// Type Aliases
// ============================================================================

// Native pointer type - koffi returns opaque handles
type NativePointer = unknown;

// ============================================================================
// Native Library Loading
// ============================================================================

let lib: koffi.IKoffiLib | null = null;

function getLibraryPath(): string {
  const platform = process.platform;

  const __filename = fileURLToPath(import.meta.url);
  const __dirname = path.dirname(__filename);

  let libName: string;
  if (platform === 'win32') {
    libName = 'QuestPDF.Interop.dll';
  } else if (platform === 'darwin') {
    libName = 'QuestPDF.Interop.dylib';
  } else {
    libName = 'libQuestPDF.Interop.so';
  }

  // Try multiple locations
  const searchPaths = [
    path.join(__dirname, libName),
    path.join(__dirname, '..', libName),
    path.join(process.cwd(), libName),
  ];

  for (const searchPath of searchPaths) {
    if (fs.existsSync(searchPath)) {
      return searchPath;
    }
  }

  return libName; // Let koffi try to find it
}

// ============================================================================
// Type Definitions
// ============================================================================

// Buffer struct for receiving byte arrays from native code
const BufferStruct = koffi.struct('Buffer', {
  data: 'void*',
  length: 'size_t',
});

// Callback types for koffi.register()
const DocumentContainerCallbackType = koffi.proto('void documentContainerCallback(void* container)');
const DocumentContainerCallbackPtr = koffi.pointer(DocumentContainerCallbackType);
const PageDescriptorCallbackType = koffi.proto('void pageDescriptorCallback(void* page)');
const PageDescriptorCallbackPtr = koffi.pointer(PageDescriptorCallbackType);
const VoidPtrCallbackType = koffi.proto('void voidPtrCallback(void* ptr)');
const VoidPtrCallbackPtr = koffi.pointer(VoidPtrCallbackType);

// Initialization callbacks for generated classes
const classInitializers: Array<(lib: koffi.IKoffiLib) => void> = [];

// ============================================================================
// Enums
// ============================================================================


export class Color {
  public readonly hex: number;

  constructor(hexVal: number) {
    this.hex = hexVal >>> 0;
  }

  get alpha(): number {
    return (this.hex >>> 24) & 0xFF;
  }

  get red(): number {
    return (this.hex >>> 16) & 0xFF;
  }

  get green(): number {
    return (this.hex >>> 8) & 0xFF;
  }

  get blue(): number {
    return this.hex & 0xFF;
  }

  toString(): string {
    if (this.alpha === 0xFF) {
      return `#${this.red.toString(16).padStart(2, '0')}${this.green.toString(16).padStart(2, '0')}${this.blue.toString(16).padStart(2, '0')}`.toUpperCase();
    }
    return `#${this.alpha.toString(16).padStart(2, '0')}${this.red.toString(16).padStart(2, '0')}${this.green.toString(16).padStart(2, '0')}${this.blue.toString(16).padStart(2, '0')}`.toUpperCase();
  }
}

export const Colors = {

  black: new Color(0xFF000000),

  white: new Color(0xFFFFFFFF),

  transparent: new Color(0x00000000),


  red: {

    lighten5: new Color(0xFFFFEBEE),

    lighten4: new Color(0xFFFFCDD2),

    lighten3: new Color(0xFFEF9A9A),

    lighten2: new Color(0xFFE57373),

    lighten1: new Color(0xFFEF5350),

    medium: new Color(0xFFF44336),

    darken1: new Color(0xFFE53935),

    darken2: new Color(0xFFD32F2F),

    darken3: new Color(0xFFC62828),

    darken4: new Color(0xFFB71C1C),

    accent1: new Color(0xFFFF8A80),

    accent2: new Color(0xFFFF5252),

    accent3: new Color(0xFFFF1744),

    accent4: new Color(0xFFD50000),

  },

  pink: {

    lighten5: new Color(0xFFFCE4EC),

    lighten4: new Color(0xFFF8BBD0),

    lighten3: new Color(0xFFF48FB1),

    lighten2: new Color(0xFFF06292),

    lighten1: new Color(0xFFEC407A),

    medium: new Color(0xFFE91E63),

    darken1: new Color(0xFFD81B60),

    darken2: new Color(0xFFC2185B),

    darken3: new Color(0xFFAD1457),

    darken4: new Color(0xFF880E4F),

    accent1: new Color(0xFFFF80AB),

    accent2: new Color(0xFFFF4081),

    accent3: new Color(0xFFF50057),

    accent4: new Color(0xFFC51162),

  },

  purple: {

    lighten5: new Color(0xFFF3E5F5),

    lighten4: new Color(0xFFE1BEE7),

    lighten3: new Color(0xFFCE93D8),

    lighten2: new Color(0xFFBA68C8),

    lighten1: new Color(0xFFAB47BC),

    medium: new Color(0xFF9C27B0),

    darken1: new Color(0xFF8E24AA),

    darken2: new Color(0xFF7B1FA2),

    darken3: new Color(0xFF6A1B9A),

    darken4: new Color(0xFF4A148C),

    accent1: new Color(0xFFEA80FC),

    accent2: new Color(0xFFE040FB),

    accent3: new Color(0xFFD500F9),

    accent4: new Color(0xFFAA00FF),

  },

  deepPurple: {

    lighten5: new Color(0xFFEDE7F6),

    lighten4: new Color(0xFFD1C4E9),

    lighten3: new Color(0xFFB39DDB),

    lighten2: new Color(0xFF9575CD),

    lighten1: new Color(0xFF7E57C2),

    medium: new Color(0xFF673AB7),

    darken1: new Color(0xFF5E35B1),

    darken2: new Color(0xFF512DA8),

    darken3: new Color(0xFF4527A0),

    darken4: new Color(0xFF311B92),

    accent1: new Color(0xFFB388FF),

    accent2: new Color(0xFF7C4DFF),

    accent3: new Color(0xFF651FFF),

    accent4: new Color(0xFF6200EA),

  },

  indigo: {

    lighten5: new Color(0xFFE8EAF6),

    lighten4: new Color(0xFFC5CAE9),

    lighten3: new Color(0xFF9FA8DA),

    lighten2: new Color(0xFF7986CB),

    lighten1: new Color(0xFF5C6BC0),

    medium: new Color(0xFF3F51B5),

    darken1: new Color(0xFF3949AB),

    darken2: new Color(0xFF303F9F),

    darken3: new Color(0xFF283593),

    darken4: new Color(0xFF1A237E),

    accent1: new Color(0xFF8C9EFF),

    accent2: new Color(0xFF536DFE),

    accent3: new Color(0xFF3D5AFE),

    accent4: new Color(0xFF304FFE),

  },

  blue: {

    lighten5: new Color(0xFFE3F2FD),

    lighten4: new Color(0xFFBBDEFB),

    lighten3: new Color(0xFF90CAF9),

    lighten2: new Color(0xFF64B5F6),

    lighten1: new Color(0xFF42A5F5),

    medium: new Color(0xFF2196F3),

    darken1: new Color(0xFF1E88E5),

    darken2: new Color(0xFF1976D2),

    darken3: new Color(0xFF1565C0),

    darken4: new Color(0xFF0D47A1),

    accent1: new Color(0xFF82B1FF),

    accent2: new Color(0xFF448AFF),

    accent3: new Color(0xFF2979FF),

    accent4: new Color(0xFF2962FF),

  },

  lightBlue: {

    lighten5: new Color(0xFFE1F5FE),

    lighten4: new Color(0xFFB3E5FC),

    lighten3: new Color(0xFF81D4FA),

    lighten2: new Color(0xFF4FC3F7),

    lighten1: new Color(0xFF29B6F6),

    medium: new Color(0xFF03A9F4),

    darken1: new Color(0xFF039BE5),

    darken2: new Color(0xFF0288D1),

    darken3: new Color(0xFF0277BD),

    darken4: new Color(0xFF01579B),

    accent1: new Color(0xFF80D8FF),

    accent2: new Color(0xFF40C4FF),

    accent3: new Color(0xFF00B0FF),

    accent4: new Color(0xFF0091EA),

  },

  cyan: {

    lighten5: new Color(0xFFE0F7FA),

    lighten4: new Color(0xFFB2EBF2),

    lighten3: new Color(0xFF80DEEA),

    lighten2: new Color(0xFF4DD0E1),

    lighten1: new Color(0xFF26C6DA),

    medium: new Color(0xFF00BCD4),

    darken1: new Color(0xFF00ACC1),

    darken2: new Color(0xFF0097A7),

    darken3: new Color(0xFF00838F),

    darken4: new Color(0xFF006064),

    accent1: new Color(0xFF84FFFF),

    accent2: new Color(0xFF18FFFF),

    accent3: new Color(0xFF00E5FF),

    accent4: new Color(0xFF00B8D4),

  },

  teal: {

    lighten5: new Color(0xFFE0F2F1),

    lighten4: new Color(0xFFB2DFDB),

    lighten3: new Color(0xFF80CBC4),

    lighten2: new Color(0xFF4DB6AC),

    lighten1: new Color(0xFF26A69A),

    medium: new Color(0xFF009688),

    darken1: new Color(0xFF00897B),

    darken2: new Color(0xFF00796B),

    darken3: new Color(0xFF00695C),

    darken4: new Color(0xFF004D40),

    accent1: new Color(0xFFA7FFEB),

    accent2: new Color(0xFF64FFDA),

    accent3: new Color(0xFF1DE9B6),

    accent4: new Color(0xFF00BFA5),

  },

  green: {

    lighten5: new Color(0xFFE8F5E9),

    lighten4: new Color(0xFFC8E6C9),

    lighten3: new Color(0xFFA5D6A7),

    lighten2: new Color(0xFF81C784),

    lighten1: new Color(0xFF66BB6A),

    medium: new Color(0xFF4CAF50),

    darken1: new Color(0xFF43A047),

    darken2: new Color(0xFF388E3C),

    darken3: new Color(0xFF2E7D32),

    darken4: new Color(0xFF1B5E20),

    accent1: new Color(0xFFB9F6CA),

    accent2: new Color(0xFF69F0AE),

    accent3: new Color(0xFF00E676),

    accent4: new Color(0xFF00C853),

  },

  lightGreen: {

    lighten5: new Color(0xFFF1F8E9),

    lighten4: new Color(0xFFDCEDC8),

    lighten3: new Color(0xFFC5E1A5),

    lighten2: new Color(0xFFAED581),

    lighten1: new Color(0xFF9CCC65),

    medium: new Color(0xFF8BC34A),

    darken1: new Color(0xFF7CB342),

    darken2: new Color(0xFF689F38),

    darken3: new Color(0xFF558B2F),

    darken4: new Color(0xFF33691E),

    accent1: new Color(0xFFCCFF90),

    accent2: new Color(0xFFB2FF59),

    accent3: new Color(0xFF76FF03),

    accent4: new Color(0xFF64DD17),

  },

  lime: {

    lighten5: new Color(0xFFF9FBE7),

    lighten4: new Color(0xFFF0F4C3),

    lighten3: new Color(0xFFE6EE9C),

    lighten2: new Color(0xFFDCE775),

    lighten1: new Color(0xFFD4E157),

    medium: new Color(0xFFCDDC39),

    darken1: new Color(0xFFC0CA33),

    darken2: new Color(0xFFAFB42B),

    darken3: new Color(0xFF9E9D24),

    darken4: new Color(0xFF827717),

    accent1: new Color(0xFFF4FF81),

    accent2: new Color(0xFFEEFF41),

    accent3: new Color(0xFFC6FF00),

    accent4: new Color(0xFFAEEA00),

  },

  yellow: {

    lighten5: new Color(0xFFFFFDE7),

    lighten4: new Color(0xFFFFF9C4),

    lighten3: new Color(0xFFFFF59D),

    lighten2: new Color(0xFFFFF176),

    lighten1: new Color(0xFFFFEE58),

    medium: new Color(0xFFFFEB3B),

    darken1: new Color(0xFFFDD835),

    darken2: new Color(0xFFFBC02D),

    darken3: new Color(0xFFF9A825),

    darken4: new Color(0xFFF57F17),

    accent1: new Color(0xFFFFFF8D),

    accent2: new Color(0xFFFFFF00),

    accent3: new Color(0xFFFFEA00),

    accent4: new Color(0xFFFFD600),

  },

  amber: {

    lighten5: new Color(0xFFFFF8E1),

    lighten4: new Color(0xFFFFECB3),

    lighten3: new Color(0xFFFFE082),

    lighten2: new Color(0xFFFFD54F),

    lighten1: new Color(0xFFFFCA28),

    medium: new Color(0xFFFFC107),

    darken1: new Color(0xFFFFB300),

    darken2: new Color(0xFFFFA000),

    darken3: new Color(0xFFFF8F00),

    darken4: new Color(0xFFFF6F00),

    accent1: new Color(0xFFFFE57F),

    accent2: new Color(0xFFFFD740),

    accent3: new Color(0xFFFFC400),

    accent4: new Color(0xFFFFAB00),

  },

  orange: {

    lighten5: new Color(0xFFFFF3E0),

    lighten4: new Color(0xFFFFE0B2),

    lighten3: new Color(0xFFFFCC80),

    lighten2: new Color(0xFFFFB74D),

    lighten1: new Color(0xFFFFA726),

    medium: new Color(0xFFFF9800),

    darken1: new Color(0xFFFB8C00),

    darken2: new Color(0xFFF57C00),

    darken3: new Color(0xFFEF6C00),

    darken4: new Color(0xFFE65100),

    accent1: new Color(0xFFFFD180),

    accent2: new Color(0xFFFFAB40),

    accent3: new Color(0xFFFF9100),

    accent4: new Color(0xFFFF6D00),

  },

  deepOrange: {

    lighten5: new Color(0xFFFBE9E7),

    lighten4: new Color(0xFFFFCCBC),

    lighten3: new Color(0xFFFFAB91),

    lighten2: new Color(0xFFFF8A65),

    lighten1: new Color(0xFFFF7043),

    medium: new Color(0xFFFF5722),

    darken1: new Color(0xFFF4511E),

    darken2: new Color(0xFFE64A19),

    darken3: new Color(0xFFD84315),

    darken4: new Color(0xFFBF360C),

    accent1: new Color(0xFFFF9E80),

    accent2: new Color(0xFFFF6E40),

    accent3: new Color(0xFFFF3D00),

    accent4: new Color(0xFFDD2C00),

  },

  brown: {

    lighten5: new Color(0xFFEFEBE9),

    lighten4: new Color(0xFFD7CCC8),

    lighten3: new Color(0xFFBCAAA4),

    lighten2: new Color(0xFFA1887F),

    lighten1: new Color(0xFF8D6E63),

    medium: new Color(0xFF795548),

    darken1: new Color(0xFF6D4C41),

    darken2: new Color(0xFF5D4037),

    darken3: new Color(0xFF4E342E),

    darken4: new Color(0xFF3E2723),

  },

  grey: {

    lighten5: new Color(0xFFFAFAFA),

    lighten4: new Color(0xFFF5F5F5),

    lighten3: new Color(0xFFEEEEEE),

    lighten2: new Color(0xFFE0E0E0),

    lighten1: new Color(0xFFBDBDBD),

    medium: new Color(0xFF9E9E9E),

    darken1: new Color(0xFF757575),

    darken2: new Color(0xFF616161),

    darken3: new Color(0xFF424242),

    darken4: new Color(0xFF212121),

  },

  blueGrey: {

    lighten5: new Color(0xFFECEFF1),

    lighten4: new Color(0xFFCFD8DC),

    lighten3: new Color(0xFFB0BEC5),

    lighten2: new Color(0xFF90A4AE),

    lighten1: new Color(0xFF78909C),

    medium: new Color(0xFF607D8B),

    darken1: new Color(0xFF546E7A),

    darken2: new Color(0xFF455A64),

    darken3: new Color(0xFF37474F),

    darken4: new Color(0xFF263238),

  },

} as const;



export enum AspectRatioOption {
  FitWidth = 0,
  FitHeight = 1,
  FitArea = 2,
}


export enum ContentDirection {
  LeftToRight = 0,
  RightToLeft = 1,
}


export enum FontWeight {
  Thin = 100,
  ExtraLight = 200,
  Light = 300,
  Normal = 400,
  Medium = 500,
  SemiBold = 600,
  Bold = 700,
  ExtraBold = 800,
  Black = 900,
  ExtraBlack = 1000,
}


export enum ImageCompressionQuality {
  Best = 0,
  VeryHigh = 1,
  High = 2,
  Medium = 3,
  Low = 4,
  VeryLow = 5,
}


export enum ImageFormat {
  Jpeg = 0,
  Png = 1,
  Webp = 2,
}


export enum ImageScaling {
  FitWidth = 0,
  FitHeight = 1,
  FitArea = 2,
  Resize = 3,
}


export enum LicenseType {
  Community = 0,
  Professional = 1,
  Enterprise = 2,
}


export enum TextHorizontalAlignment {
  Left = 0,
  Center = 1,
  Right = 2,
  Justify = 3,
  Start = 4,
  End = 5,
}


export enum TextInjectedElementAlignment {
  AboveBaseline = 0,
  BelowBaseline = 1,
  Top = 2,
  Bottom = 3,
  Middle = 4,
}


export enum Unit {
  Point = 0,
  Meter = 1,
  Centimetre = 2,
  Millimetre = 3,
  Feet = 4,
  Inch = 5,
  Mil = 6,
}



interface SettingsNativeFunctions {
  questpdf__settings__license(value: number): void;
  questpdf__settings__enable_debugging(value: boolean): void;
  questpdf__settings__enable_caching(value: boolean): void;
  questpdf__settings__check_if_all_text_glyphs_are_available(value: boolean): void;
  questpdf__settings__use_environment_fonts(value: boolean): void;
  questpdf__settings__temporary_storage_path(value: string): void;
  questpdf__settings__font_discovery_paths(delimitedList: string): void;
}

let settingsLib: SettingsNativeFunctions | null = null;

classInitializers.push((lib: koffi.IKoffiLib) => {
  settingsLib = {
    questpdf__settings__license: lib.func('void questpdf__settings__license(int value)'),
    questpdf__settings__enable_debugging: lib.func('void questpdf__settings__enable_debugging(bool value)'),
    questpdf__settings__enable_caching: lib.func('void questpdf__settings__enable_caching(bool value)'),
    questpdf__settings__check_if_all_text_glyphs_are_available: lib.func('void questpdf__settings__check_if_all_text_glyphs_are_available(bool value)'),
    questpdf__settings__use_environment_fonts: lib.func('void questpdf__settings__use_environment_fonts(bool value)'),
    questpdf__settings__temporary_storage_path: lib.func('void questpdf__settings__temporary_storage_path(const char* value)'),
    questpdf__settings__font_discovery_paths: lib.func('void questpdf__settings__font_discovery_paths(const char* delimitedList)'),
  };
});

function getSettingsLib(): SettingsNativeFunctions {
  if (settingsLib === null) {
    throw new Error('QuestPDF library not initialized. Call initializeQuestPDF() first.');
  }
  return settingsLib;
}

export class Settings {
  static set license(value: LicenseType | null) {
    if (value === null) return;

    getSettingsLib().questpdf__settings__license(value);
  }

  static set enableDebugging(value: boolean) {
    getSettingsLib().questpdf__settings__enable_debugging(value);
  }

  static set enableCaching(value: boolean) {
    getSettingsLib().questpdf__settings__enable_caching(value);
  }

  static set checkIfAllTextGlyphsAreAvailable(value: boolean) {
    getSettingsLib().questpdf__settings__check_if_all_text_glyphs_are_available(value);
  }

  static set useEnvironmentFonts(value: boolean) {
    getSettingsLib().questpdf__settings__use_environment_fonts(value);
  }

  static set temporaryStoragePath(value: string) {
    getSettingsLib().questpdf__settings__temporary_storage_path(value);
  }

  static set fontDiscoveryPaths(value: string[]) {
    getSettingsLib().questpdf__settings__font_discovery_paths(value.join('__questpdf__'));
  }
}




// ============================================================================
// FontFeatures - Native Function Declarations
// ============================================================================





const FontFeaturesNativeFunctions: Record<string, ((...args: any[]) => any) | null> = {};

function initFontFeaturesFunctions(lib: koffi.IKoffiLib): void {

  FontFeaturesNativeFunctions['questpdf__font_features__character_variant__3a2f9f96'] = lib.func('str16 questpdf__font_features__character_variant__3a2f9f96(int32_t value)');

  FontFeaturesNativeFunctions['questpdf__font_features__stylistic_set__19e9e157'] = lib.func('str16 questpdf__font_features__stylistic_set__19e9e157(int32_t value)');



}

// Register initialization with the global initializers
classInitializers.push((lib: koffi.IKoffiLib) => initFontFeaturesFunctions(lib));

// ============================================================================
// FontFeatures Class
// ============================================================================

export class FontFeatures {
  private readonly _ptr: NativePointer;
  private readonly _callbacks: koffi.IKoffiRegisteredCallback[] = [];

  constructor(ptr: NativePointer) {
    this._ptr = ptr;
  }

  get pointer(): NativePointer {
    return this._ptr;
  }


  
    static characterVariant(value: number): string {


    const result = FontFeaturesNativeFunctions['questpdf__font_features__character_variant__3a2f9f96']!(value);

    return new string(result);

  }


  
    static stylisticSet(value: number): string {


    const result = FontFeaturesNativeFunctions['questpdf__font_features__stylistic_set__19e9e157']!(value);

    return new string(result);

  }




}


// ============================================================================
// Placeholders - Native Function Declarations
// ============================================================================





const PlaceholdersNativeFunctions: Record<string, ((...args: any[]) => any) | null> = {};

function initPlaceholdersFunctions(lib: koffi.IKoffiLib): void {

  PlaceholdersNativeFunctions['questpdf__placeholders__lorem_ipsum__39e2dfef'] = lib.func('str16 questpdf__placeholders__lorem_ipsum__39e2dfef()');

  PlaceholdersNativeFunctions['questpdf__placeholders__label__06cbd381'] = lib.func('str16 questpdf__placeholders__label__06cbd381()');

  PlaceholdersNativeFunctions['questpdf__placeholders__sentence__d64f6e68'] = lib.func('str16 questpdf__placeholders__sentence__d64f6e68()');

  PlaceholdersNativeFunctions['questpdf__placeholders__question__21de659f'] = lib.func('str16 questpdf__placeholders__question__21de659f()');

  PlaceholdersNativeFunctions['questpdf__placeholders__paragraph__56235510'] = lib.func('str16 questpdf__placeholders__paragraph__56235510()');

  PlaceholdersNativeFunctions['questpdf__placeholders__paragraphs__addb8834'] = lib.func('str16 questpdf__placeholders__paragraphs__addb8834()');

  PlaceholdersNativeFunctions['questpdf__placeholders__email__e6405590'] = lib.func('str16 questpdf__placeholders__email__e6405590()');

  PlaceholdersNativeFunctions['questpdf__placeholders__name__e536835d'] = lib.func('str16 questpdf__placeholders__name__e536835d()');

  PlaceholdersNativeFunctions['questpdf__placeholders__phone_number__25314f3f'] = lib.func('str16 questpdf__placeholders__phone_number__25314f3f()');

  PlaceholdersNativeFunctions['questpdf__placeholders__webpage_url__6e903669'] = lib.func('str16 questpdf__placeholders__webpage_url__6e903669()');

  PlaceholdersNativeFunctions['questpdf__placeholders__price__0ae34c02'] = lib.func('str16 questpdf__placeholders__price__0ae34c02()');

  PlaceholdersNativeFunctions['questpdf__placeholders__time__af3e33a8'] = lib.func('str16 questpdf__placeholders__time__af3e33a8()');

  PlaceholdersNativeFunctions['questpdf__placeholders__short_date__f6b7ade4'] = lib.func('str16 questpdf__placeholders__short_date__f6b7ade4()');

  PlaceholdersNativeFunctions['questpdf__placeholders__long_date__736f3796'] = lib.func('str16 questpdf__placeholders__long_date__736f3796()');

  PlaceholdersNativeFunctions['questpdf__placeholders__date_time__b29d44f5'] = lib.func('str16 questpdf__placeholders__date_time__b29d44f5()');

  PlaceholdersNativeFunctions['questpdf__placeholders__integer__e8bfa000'] = lib.func('str16 questpdf__placeholders__integer__e8bfa000()');

  PlaceholdersNativeFunctions['questpdf__placeholders__decimal__4569a5a1'] = lib.func('str16 questpdf__placeholders__decimal__4569a5a1()');

  PlaceholdersNativeFunctions['questpdf__placeholders__percent__3fa245e1'] = lib.func('str16 questpdf__placeholders__percent__3fa245e1()');

  PlaceholdersNativeFunctions['questpdf__placeholders__background_color__5395a374'] = lib.func('uint32_t questpdf__placeholders__background_color__5395a374()');

  PlaceholdersNativeFunctions['questpdf__placeholders__color__40d13de0'] = lib.func('uint32_t questpdf__placeholders__color__40d13de0()');

  PlaceholdersNativeFunctions['questpdf__placeholders__image__a8827df6'] = lib.func('void* questpdf__placeholders__image__a8827df6(int32_t width, int32_t height)');

  PlaceholdersNativeFunctions['questpdf__placeholders__image__2891dd9e'] = lib.func('void* questpdf__placeholders__image__2891dd9e(void* size)');



}

// Register initialization with the global initializers
classInitializers.push((lib: koffi.IKoffiLib) => initPlaceholdersFunctions(lib));

// ============================================================================
// Placeholders Class
// ============================================================================

export class Placeholders {
  private readonly _ptr: NativePointer;
  private readonly _callbacks: koffi.IKoffiRegisteredCallback[] = [];

  constructor(ptr: NativePointer) {
    this._ptr = ptr;
  }

  get pointer(): NativePointer {
    return this._ptr;
  }


  
    static loremIpsum(): string {


    const result = PlaceholdersNativeFunctions['questpdf__placeholders__lorem_ipsum__39e2dfef']!();

    return new string(result);

  }


  
    static label(): string {


    const result = PlaceholdersNativeFunctions['questpdf__placeholders__label__06cbd381']!();

    return new string(result);

  }


  
    static sentence(): string {


    const result = PlaceholdersNativeFunctions['questpdf__placeholders__sentence__d64f6e68']!();

    return new string(result);

  }


  
    static question(): string {


    const result = PlaceholdersNativeFunctions['questpdf__placeholders__question__21de659f']!();

    return new string(result);

  }


  
    static paragraph(): string {


    const result = PlaceholdersNativeFunctions['questpdf__placeholders__paragraph__56235510']!();

    return new string(result);

  }


  
    static paragraphs(): string {


    const result = PlaceholdersNativeFunctions['questpdf__placeholders__paragraphs__addb8834']!();

    return new string(result);

  }


  
    static email(): string {


    const result = PlaceholdersNativeFunctions['questpdf__placeholders__email__e6405590']!();

    return new string(result);

  }


  
    static name(): string {


    const result = PlaceholdersNativeFunctions['questpdf__placeholders__name__e536835d']!();

    return new string(result);

  }


  
    static phoneNumber(): string {


    const result = PlaceholdersNativeFunctions['questpdf__placeholders__phone_number__25314f3f']!();

    return new string(result);

  }


  
    static webpageUrl(): string {


    const result = PlaceholdersNativeFunctions['questpdf__placeholders__webpage_url__6e903669']!();

    return new string(result);

  }


  
    static price(): string {


    const result = PlaceholdersNativeFunctions['questpdf__placeholders__price__0ae34c02']!();

    return new string(result);

  }


  
    static time(): string {


    const result = PlaceholdersNativeFunctions['questpdf__placeholders__time__af3e33a8']!();

    return new string(result);

  }


  
    static shortDate(): string {


    const result = PlaceholdersNativeFunctions['questpdf__placeholders__short_date__f6b7ade4']!();

    return new string(result);

  }


  
    static longDate(): string {


    const result = PlaceholdersNativeFunctions['questpdf__placeholders__long_date__736f3796']!();

    return new string(result);

  }


  
    static dateTime(): string {


    const result = PlaceholdersNativeFunctions['questpdf__placeholders__date_time__b29d44f5']!();

    return new string(result);

  }


  
    static integer(): string {


    const result = PlaceholdersNativeFunctions['questpdf__placeholders__integer__e8bfa000']!();

    return new string(result);

  }


  
    static decimal(): string {


    const result = PlaceholdersNativeFunctions['questpdf__placeholders__decimal__4569a5a1']!();

    return new string(result);

  }


  
    static percent(): string {


    const result = PlaceholdersNativeFunctions['questpdf__placeholders__percent__3fa245e1']!();

    return new string(result);

  }


  
    static backgroundColor(): Color {


    const result = PlaceholdersNativeFunctions['questpdf__placeholders__background_color__5395a374']!();

    return new Color(result);

  }


  
    static color(): Color {


    const result = PlaceholdersNativeFunctions['questpdf__placeholders__color__40d13de0']!();

    return new Color(result);

  }


  
    static private image_Int_Int(width: number, height: number): unknown {


    const result = PlaceholdersNativeFunctions['questpdf__placeholders__image__a8827df6']!(width, height);

    return new unknown(result);

  }


  
    static private image_ImageSize(size: unknown): unknown {


    const result = PlaceholdersNativeFunctions['questpdf__placeholders__image__2891dd9e']!(size);

    return new unknown(result);

  }




}


// ============================================================================
// FontManager - Native Function Declarations
// ============================================================================





const FontManagerNativeFunctions: Record<string, ((...args: any[]) => any) | null> = {};

function initFontManagerFunctions(lib: koffi.IKoffiLib): void {



}

// Register initialization with the global initializers
classInitializers.push((lib: koffi.IKoffiLib) => initFontManagerFunctions(lib));

// ============================================================================
// FontManager Class
// ============================================================================

export class FontManager {
  private readonly _ptr: NativePointer;
  private readonly _callbacks: koffi.IKoffiRegisteredCallback[] = [];

  constructor(ptr: NativePointer) {
    this._ptr = ptr;
  }

  get pointer(): NativePointer {
    return this._ptr;
  }




}


// ============================================================================
// Image - Native Function Declarations
// ============================================================================





const ImageNativeFunctions: Record<string, ((...args: any[]) => any) | null> = {};

function initImageFunctions(lib: koffi.IKoffiLib): void {

  ImageNativeFunctions['questpdf__image__from_file__d83c4447'] = lib.func('void* questpdf__image__from_file__d83c4447(str16 filePath)');



}

// Register initialization with the global initializers
classInitializers.push((lib: koffi.IKoffiLib) => initImageFunctions(lib));

// ============================================================================
// Image Class
// ============================================================================

export class Image {
  private readonly _ptr: NativePointer;
  private readonly _callbacks: koffi.IKoffiRegisteredCallback[] = [];

  constructor(ptr: NativePointer) {
    this._ptr = ptr;
  }

  get pointer(): NativePointer {
    return this._ptr;
  }


  
    static fromFile(filePath: string): Image {


    const result = ImageNativeFunctions['questpdf__image__from_file__d83c4447']!(filePath);

    return new Image(result);

  }




}


// ============================================================================
// SvgImage - Native Function Declarations
// ============================================================================





const SvgImageNativeFunctions: Record<string, ((...args: any[]) => any) | null> = {};

function initSvgImageFunctions(lib: koffi.IKoffiLib): void {

  SvgImageNativeFunctions['questpdf__svg_image__from_file__79e2c64d'] = lib.func('void* questpdf__svg_image__from_file__79e2c64d(str16 filePath)');

  SvgImageNativeFunctions['questpdf__svg_image__from_text__82b4cf0f'] = lib.func('void* questpdf__svg_image__from_text__82b4cf0f(str16 svg)');



}

// Register initialization with the global initializers
classInitializers.push((lib: koffi.IKoffiLib) => initSvgImageFunctions(lib));

// ============================================================================
// SvgImage Class
// ============================================================================

export class SvgImage {
  private readonly _ptr: NativePointer;
  private readonly _callbacks: koffi.IKoffiRegisteredCallback[] = [];

  constructor(ptr: NativePointer) {
    this._ptr = ptr;
  }

  get pointer(): NativePointer {
    return this._ptr;
  }


  
    static fromFile(filePath: string): SvgImage {


    const result = SvgImageNativeFunctions['questpdf__svg_image__from_file__79e2c64d']!(filePath);

    return new SvgImage(result);

  }


  
    static fromText(svg: string): SvgImage {


    const result = SvgImageNativeFunctions['questpdf__svg_image__from_text__82b4cf0f']!(svg);

    return new SvgImage(result);

  }




}


// ============================================================================
// LineDescriptor - Native Function Declarations
// ============================================================================





const LineDescriptorNativeFunctions: Record<string, ((...args: any[]) => any) | null> = {};

function initLineDescriptorFunctions(lib: koffi.IKoffiLib): void {

  LineDescriptorNativeFunctions['questpdf__line_descriptor__line_color__a86ca4e3'] = lib.func('void* questpdf__line_descriptor__line_color__a86ca4e3(void* target, uint32_t color)');


LineDescriptorNativeFunctions['questpdf__line_descriptor__line_dash_pattern'] = lib.func('void* questpdf__line_descriptor__line_dash_pattern(void* target, float* values, int valuesLength, int unit)');
LineDescriptorNativeFunctions['questpdf__line_descriptor__line_gradient'] = lib.func('void* questpdf__line_descriptor__line_gradient(void* target, uint32_t* colors, int colorsLength)');

}

// Register initialization with the global initializers
classInitializers.push((lib: koffi.IKoffiLib) => initLineDescriptorFunctions(lib));

// ============================================================================
// LineDescriptor Class
// ============================================================================

export class LineDescriptor {
  private readonly _ptr: NativePointer;
  private readonly _callbacks: koffi.IKoffiRegisteredCallback[] = [];

  constructor(ptr: NativePointer) {
    this._ptr = ptr;
  }

  get pointer(): NativePointer {
    return this._ptr;
  }


  
    lineColor(color: Color): LineDescriptor {


    const result = LineDescriptorNativeFunctions['questpdf__line_descriptor__line_color__a86ca4e3']!(this._ptr, color.hex);

    return new LineDescriptor(result);

  }



lineDashPattern(pattern: number[], unit: Unit = Unit.Point): LineDescriptor {
    const arr = new Float32Array(pattern);
    const result = LineDescriptorNativeFunctions['questpdf__line_descriptor__line_dash_pattern']!(this._ptr, arr, arr.length, unit);
    return new LineDescriptor(result);
}

lineGradient(colors: Color[]): LineDescriptor {
    const arr = new Uint32Array(colors.map(x => x.hex));
    const result = LineDescriptorNativeFunctions['questpdf__line_descriptor__line_gradient']!(this._ptr, arr, arr.length);
    return new LineDescriptor(result);
}

}


// ============================================================================
// ColumnDescriptor - Native Function Declarations
// ============================================================================





const ColumnDescriptorNativeFunctions: Record<string, ((...args: any[]) => any) | null> = {};

function initColumnDescriptorFunctions(lib: koffi.IKoffiLib): void {

  ColumnDescriptorNativeFunctions['questpdf__column_descriptor__spacing__e47553e3'] = lib.func('void questpdf__column_descriptor__spacing__e47553e3(void* target, float value, int32_t unit)');

  ColumnDescriptorNativeFunctions['questpdf__column_descriptor__item__2cf2ad89'] = lib.func('void* questpdf__column_descriptor__item__2cf2ad89(void* target)');



}

// Register initialization with the global initializers
classInitializers.push((lib: koffi.IKoffiLib) => initColumnDescriptorFunctions(lib));

// ============================================================================
// ColumnDescriptor Class
// ============================================================================

export class ColumnDescriptor {
  private readonly _ptr: NativePointer;
  private readonly _callbacks: koffi.IKoffiRegisteredCallback[] = [];

  constructor(ptr: NativePointer) {
    this._ptr = ptr;
  }

  get pointer(): NativePointer {
    return this._ptr;
  }


  
    spacing(value: number, unit: Unit = Unit.Point): void {


    const result = ColumnDescriptorNativeFunctions['questpdf__column_descriptor__spacing__e47553e3']!(this._ptr, value, unit);

  }


  
    item(): Container {


    const result = ColumnDescriptorNativeFunctions['questpdf__column_descriptor__item__2cf2ad89']!(this._ptr);

    return new Container(result);

  }




}


// ============================================================================
// DecorationDescriptor - Native Function Declarations
// ============================================================================




// typedef void (*container_callback_bf5ce29e)(void*);

// typedef void (*container_callback_391a971a)(void*);

// typedef void (*container_callback_4c35dd57)(void*);


const DecorationDescriptorNativeFunctions: Record<string, ((...args: any[]) => any) | null> = {};

function initDecorationDescriptorFunctions(lib: koffi.IKoffiLib): void {

  DecorationDescriptorNativeFunctions['questpdf__decoration_descriptor__before__1bfecdf8'] = lib.func('void* questpdf__decoration_descriptor__before__1bfecdf8(void* target)');

  DecorationDescriptorNativeFunctions['questpdf__decoration_descriptor__before__bf5ce29e'] = lib.func('void questpdf__decoration_descriptor__before__bf5ce29e(void* target, void* handler)');

  DecorationDescriptorNativeFunctions['questpdf__decoration_descriptor__content__9ec35667'] = lib.func('void* questpdf__decoration_descriptor__content__9ec35667(void* target)');

  DecorationDescriptorNativeFunctions['questpdf__decoration_descriptor__content__391a971a'] = lib.func('void questpdf__decoration_descriptor__content__391a971a(void* target, void* handler)');

  DecorationDescriptorNativeFunctions['questpdf__decoration_descriptor__after__4cf66f67'] = lib.func('void* questpdf__decoration_descriptor__after__4cf66f67(void* target)');

  DecorationDescriptorNativeFunctions['questpdf__decoration_descriptor__after__4c35dd57'] = lib.func('void questpdf__decoration_descriptor__after__4c35dd57(void* target, void* handler)');



}

// Register initialization with the global initializers
classInitializers.push((lib: koffi.IKoffiLib) => initDecorationDescriptorFunctions(lib));

// ============================================================================
// DecorationDescriptor Class
// ============================================================================

export class DecorationDescriptor {
  private readonly _ptr: NativePointer;
  private readonly _callbacks: koffi.IKoffiRegisteredCallback[] = [];

  constructor(ptr: NativePointer) {
    this._ptr = ptr;
  }

  get pointer(): NativePointer {
    return this._ptr;
  }


  
    private before_NoArgs(): Container {


    const result = DecorationDescriptorNativeFunctions['questpdf__decoration_descriptor__before__1bfecdf8']!(this._ptr);

    return new Container(result);

  }


  
    private before_Action(handler: (arg0: Container) => void): void {

    // Create callback type and register handler
    const handlerCallbackType_bf5ce29e = koffi.proto('void handlerCb_bf5ce29e(void* ptr)');
    const handlerCallbackPtr_bf5ce29e = koffi.pointer(handlerCallbackType_bf5ce29e);
    const handlerCb = koffi.register((ptr: NativePointer) => {
      const obj = new Container(ptr);
      handler(obj);
    }, handlerCallbackPtr_bf5ce29e);
    this._callbacks.push(handlerCb);


    const result = DecorationDescriptorNativeFunctions['questpdf__decoration_descriptor__before__bf5ce29e']!(this._ptr, handlerCb);

  }


  
    private content_NoArgs(): Container {


    const result = DecorationDescriptorNativeFunctions['questpdf__decoration_descriptor__content__9ec35667']!(this._ptr);

    return new Container(result);

  }


  
    private content_Action(handler: (arg0: Container) => void): void {

    // Create callback type and register handler
    const handlerCallbackType_391a971a = koffi.proto('void handlerCb_391a971a(void* ptr)');
    const handlerCallbackPtr_391a971a = koffi.pointer(handlerCallbackType_391a971a);
    const handlerCb = koffi.register((ptr: NativePointer) => {
      const obj = new Container(ptr);
      handler(obj);
    }, handlerCallbackPtr_391a971a);
    this._callbacks.push(handlerCb);


    const result = DecorationDescriptorNativeFunctions['questpdf__decoration_descriptor__content__391a971a']!(this._ptr, handlerCb);

  }


  
    private after_NoArgs(): Container {


    const result = DecorationDescriptorNativeFunctions['questpdf__decoration_descriptor__after__4cf66f67']!(this._ptr);

    return new Container(result);

  }


  
    private after_Action(handler: (arg0: Container) => void): void {

    // Create callback type and register handler
    const handlerCallbackType_4c35dd57 = koffi.proto('void handlerCb_4c35dd57(void* ptr)');
    const handlerCallbackPtr_4c35dd57 = koffi.pointer(handlerCallbackType_4c35dd57);
    const handlerCb = koffi.register((ptr: NativePointer) => {
      const obj = new Container(ptr);
      handler(obj);
    }, handlerCallbackPtr_4c35dd57);
    this._callbacks.push(handlerCb);


    const result = DecorationDescriptorNativeFunctions['questpdf__decoration_descriptor__after__4c35dd57']!(this._ptr, handlerCb);

  }




}


// ============================================================================
// InlinedDescriptor - Native Function Declarations
// ============================================================================





const InlinedDescriptorNativeFunctions: Record<string, ((...args: any[]) => any) | null> = {};

function initInlinedDescriptorFunctions(lib: koffi.IKoffiLib): void {

  InlinedDescriptorNativeFunctions['questpdf__inlined_descriptor__spacing__e466eaa7'] = lib.func('void questpdf__inlined_descriptor__spacing__e466eaa7(void* target, float value, int32_t unit)');

  InlinedDescriptorNativeFunctions['questpdf__inlined_descriptor__vertical_spacing__44456280'] = lib.func('void questpdf__inlined_descriptor__vertical_spacing__44456280(void* target, float value, int32_t unit)');

  InlinedDescriptorNativeFunctions['questpdf__inlined_descriptor__horizontal_spacing__a035fbb4'] = lib.func('void questpdf__inlined_descriptor__horizontal_spacing__a035fbb4(void* target, float value, int32_t unit)');

  InlinedDescriptorNativeFunctions['questpdf__inlined_descriptor__baseline_top__96b48f7f'] = lib.func('void questpdf__inlined_descriptor__baseline_top__96b48f7f(void* target)');

  InlinedDescriptorNativeFunctions['questpdf__inlined_descriptor__baseline_middle__2ee97366'] = lib.func('void questpdf__inlined_descriptor__baseline_middle__2ee97366(void* target)');

  InlinedDescriptorNativeFunctions['questpdf__inlined_descriptor__baseline_bottom__1878876e'] = lib.func('void questpdf__inlined_descriptor__baseline_bottom__1878876e(void* target)');

  InlinedDescriptorNativeFunctions['questpdf__inlined_descriptor__align_left__0c3a1762'] = lib.func('void questpdf__inlined_descriptor__align_left__0c3a1762(void* target)');

  InlinedDescriptorNativeFunctions['questpdf__inlined_descriptor__align_center__d09c92f2'] = lib.func('void questpdf__inlined_descriptor__align_center__d09c92f2(void* target)');

  InlinedDescriptorNativeFunctions['questpdf__inlined_descriptor__align_right__99b3ac01'] = lib.func('void questpdf__inlined_descriptor__align_right__99b3ac01(void* target)');

  InlinedDescriptorNativeFunctions['questpdf__inlined_descriptor__align_justify__3f036912'] = lib.func('void questpdf__inlined_descriptor__align_justify__3f036912(void* target)');

  InlinedDescriptorNativeFunctions['questpdf__inlined_descriptor__align_space_around__cfaed88d'] = lib.func('void questpdf__inlined_descriptor__align_space_around__cfaed88d(void* target)');

  InlinedDescriptorNativeFunctions['questpdf__inlined_descriptor__item__3a4e6d7b'] = lib.func('void* questpdf__inlined_descriptor__item__3a4e6d7b(void* target)');



}

// Register initialization with the global initializers
classInitializers.push((lib: koffi.IKoffiLib) => initInlinedDescriptorFunctions(lib));

// ============================================================================
// InlinedDescriptor Class
// ============================================================================

export class InlinedDescriptor {
  private readonly _ptr: NativePointer;
  private readonly _callbacks: koffi.IKoffiRegisteredCallback[] = [];

  constructor(ptr: NativePointer) {
    this._ptr = ptr;
  }

  get pointer(): NativePointer {
    return this._ptr;
  }


  
    spacing(value: number, unit: Unit = Unit.Point): void {


    const result = InlinedDescriptorNativeFunctions['questpdf__inlined_descriptor__spacing__e466eaa7']!(this._ptr, value, unit);

  }


  
    verticalSpacing(value: number, unit: Unit = Unit.Point): void {


    const result = InlinedDescriptorNativeFunctions['questpdf__inlined_descriptor__vertical_spacing__44456280']!(this._ptr, value, unit);

  }


  
    horizontalSpacing(value: number, unit: Unit = Unit.Point): void {


    const result = InlinedDescriptorNativeFunctions['questpdf__inlined_descriptor__horizontal_spacing__a035fbb4']!(this._ptr, value, unit);

  }


  
    baselineTop(): void {


    const result = InlinedDescriptorNativeFunctions['questpdf__inlined_descriptor__baseline_top__96b48f7f']!(this._ptr);

  }


  
    baselineMiddle(): void {


    const result = InlinedDescriptorNativeFunctions['questpdf__inlined_descriptor__baseline_middle__2ee97366']!(this._ptr);

  }


  
    baselineBottom(): void {


    const result = InlinedDescriptorNativeFunctions['questpdf__inlined_descriptor__baseline_bottom__1878876e']!(this._ptr);

  }


  
    alignLeft(): void {


    const result = InlinedDescriptorNativeFunctions['questpdf__inlined_descriptor__align_left__0c3a1762']!(this._ptr);

  }


  
    alignCenter(): void {


    const result = InlinedDescriptorNativeFunctions['questpdf__inlined_descriptor__align_center__d09c92f2']!(this._ptr);

  }


  
    alignRight(): void {


    const result = InlinedDescriptorNativeFunctions['questpdf__inlined_descriptor__align_right__99b3ac01']!(this._ptr);

  }


  
    alignJustify(): void {


    const result = InlinedDescriptorNativeFunctions['questpdf__inlined_descriptor__align_justify__3f036912']!(this._ptr);

  }


  
    alignSpaceAround(): void {


    const result = InlinedDescriptorNativeFunctions['questpdf__inlined_descriptor__align_space_around__cfaed88d']!(this._ptr);

  }


  
    item(): Container {


    const result = InlinedDescriptorNativeFunctions['questpdf__inlined_descriptor__item__3a4e6d7b']!(this._ptr);

    return new Container(result);

  }




}


// ============================================================================
// LayersDescriptor - Native Function Declarations
// ============================================================================





const LayersDescriptorNativeFunctions: Record<string, ((...args: any[]) => any) | null> = {};

function initLayersDescriptorFunctions(lib: koffi.IKoffiLib): void {

  LayersDescriptorNativeFunctions['questpdf__layers_descriptor__layer__f8c1dd4f'] = lib.func('void* questpdf__layers_descriptor__layer__f8c1dd4f(void* target)');

  LayersDescriptorNativeFunctions['questpdf__layers_descriptor__primary_layer__c2eb4a19'] = lib.func('void* questpdf__layers_descriptor__primary_layer__c2eb4a19(void* target)');



}

// Register initialization with the global initializers
classInitializers.push((lib: koffi.IKoffiLib) => initLayersDescriptorFunctions(lib));

// ============================================================================
// LayersDescriptor Class
// ============================================================================

export class LayersDescriptor {
  private readonly _ptr: NativePointer;
  private readonly _callbacks: koffi.IKoffiRegisteredCallback[] = [];

  constructor(ptr: NativePointer) {
    this._ptr = ptr;
  }

  get pointer(): NativePointer {
    return this._ptr;
  }


  
    layer(): Container {


    const result = LayersDescriptorNativeFunctions['questpdf__layers_descriptor__layer__f8c1dd4f']!(this._ptr);

    return new Container(result);

  }


  
    primaryLayer(): Container {


    const result = LayersDescriptorNativeFunctions['questpdf__layers_descriptor__primary_layer__c2eb4a19']!(this._ptr);

    return new Container(result);

  }




}


// ============================================================================
// RowDescriptor - Native Function Declarations
// ============================================================================





const RowDescriptorNativeFunctions: Record<string, ((...args: any[]) => any) | null> = {};

function initRowDescriptorFunctions(lib: koffi.IKoffiLib): void {

  RowDescriptorNativeFunctions['questpdf__row_descriptor__spacing__09cc7a62'] = lib.func('void questpdf__row_descriptor__spacing__09cc7a62(void* target, float spacing, int32_t unit)');

  RowDescriptorNativeFunctions['questpdf__row_descriptor__relative_item__f4570b47'] = lib.func('void* questpdf__row_descriptor__relative_item__f4570b47(void* target, float size)');

  RowDescriptorNativeFunctions['questpdf__row_descriptor__constant_item__4f927836'] = lib.func('void* questpdf__row_descriptor__constant_item__4f927836(void* target, float size, int32_t unit)');

  RowDescriptorNativeFunctions['questpdf__row_descriptor__auto_item__fc084be8'] = lib.func('void* questpdf__row_descriptor__auto_item__fc084be8(void* target)');



}

// Register initialization with the global initializers
classInitializers.push((lib: koffi.IKoffiLib) => initRowDescriptorFunctions(lib));

// ============================================================================
// RowDescriptor Class
// ============================================================================

export class RowDescriptor {
  private readonly _ptr: NativePointer;
  private readonly _callbacks: koffi.IKoffiRegisteredCallback[] = [];

  constructor(ptr: NativePointer) {
    this._ptr = ptr;
  }

  get pointer(): NativePointer {
    return this._ptr;
  }


  
    spacing(spacing: number, unit: Unit = Unit.Point): void {


    const result = RowDescriptorNativeFunctions['questpdf__row_descriptor__spacing__09cc7a62']!(this._ptr, spacing, unit);

  }


  
    relativeItem(size: number = 1): Container {


    const result = RowDescriptorNativeFunctions['questpdf__row_descriptor__relative_item__f4570b47']!(this._ptr, size);

    return new Container(result);

  }


  
    constantItem(size: number, unit: Unit = Unit.Point): Container {


    const result = RowDescriptorNativeFunctions['questpdf__row_descriptor__constant_item__4f927836']!(this._ptr, size, unit);

    return new Container(result);

  }


  
    autoItem(): Container {


    const result = RowDescriptorNativeFunctions['questpdf__row_descriptor__auto_item__fc084be8']!(this._ptr);

    return new Container(result);

  }




}


// ============================================================================
// GridDescriptor - Native Function Declarations
// ============================================================================





const GridDescriptorNativeFunctions: Record<string, ((...args: any[]) => any) | null> = {};

function initGridDescriptorFunctions(lib: koffi.IKoffiLib): void {

  GridDescriptorNativeFunctions['questpdf__grid_descriptor__spacing__2a69d201'] = lib.func('void questpdf__grid_descriptor__spacing__2a69d201(void* target, float value, int32_t unit)');

  GridDescriptorNativeFunctions['questpdf__grid_descriptor__vertical_spacing__593ca4c3'] = lib.func('void questpdf__grid_descriptor__vertical_spacing__593ca4c3(void* target, float value, int32_t unit)');

  GridDescriptorNativeFunctions['questpdf__grid_descriptor__horizontal_spacing__a9d6ceae'] = lib.func('void questpdf__grid_descriptor__horizontal_spacing__a9d6ceae(void* target, float value, int32_t unit)');

  GridDescriptorNativeFunctions['questpdf__grid_descriptor__columns__160f5f35'] = lib.func('void questpdf__grid_descriptor__columns__160f5f35(void* target, int32_t value)');

  GridDescriptorNativeFunctions['questpdf__grid_descriptor__align_left__fc5e4cb9'] = lib.func('void questpdf__grid_descriptor__align_left__fc5e4cb9(void* target)');

  GridDescriptorNativeFunctions['questpdf__grid_descriptor__align_center__3d81b2fe'] = lib.func('void questpdf__grid_descriptor__align_center__3d81b2fe(void* target)');

  GridDescriptorNativeFunctions['questpdf__grid_descriptor__align_right__e9aa71bc'] = lib.func('void questpdf__grid_descriptor__align_right__e9aa71bc(void* target)');

  GridDescriptorNativeFunctions['questpdf__grid_descriptor__item__3e7cf6ba'] = lib.func('void* questpdf__grid_descriptor__item__3e7cf6ba(void* target, int32_t columns)');



}

// Register initialization with the global initializers
classInitializers.push((lib: koffi.IKoffiLib) => initGridDescriptorFunctions(lib));

// ============================================================================
// GridDescriptor Class
// ============================================================================

export class GridDescriptor {
  private readonly _ptr: NativePointer;
  private readonly _callbacks: koffi.IKoffiRegisteredCallback[] = [];

  constructor(ptr: NativePointer) {
    this._ptr = ptr;
  }

  get pointer(): NativePointer {
    return this._ptr;
  }


  
    spacing(value: number, unit: Unit = Unit.Point): void {


    const result = GridDescriptorNativeFunctions['questpdf__grid_descriptor__spacing__2a69d201']!(this._ptr, value, unit);

  }


  
    verticalSpacing(value: number, unit: Unit = Unit.Point): void {


    const result = GridDescriptorNativeFunctions['questpdf__grid_descriptor__vertical_spacing__593ca4c3']!(this._ptr, value, unit);

  }


  
    horizontalSpacing(value: number, unit: Unit = Unit.Point): void {


    const result = GridDescriptorNativeFunctions['questpdf__grid_descriptor__horizontal_spacing__a9d6ceae']!(this._ptr, value, unit);

  }


  
    columns(value: number = 12): void {


    const result = GridDescriptorNativeFunctions['questpdf__grid_descriptor__columns__160f5f35']!(this._ptr, value);

  }


  
    alignLeft(): void {


    const result = GridDescriptorNativeFunctions['questpdf__grid_descriptor__align_left__fc5e4cb9']!(this._ptr);

  }


  
    alignCenter(): void {


    const result = GridDescriptorNativeFunctions['questpdf__grid_descriptor__align_center__3d81b2fe']!(this._ptr);

  }


  
    alignRight(): void {


    const result = GridDescriptorNativeFunctions['questpdf__grid_descriptor__align_right__e9aa71bc']!(this._ptr);

  }


  
    item(columns: number = 1): Container {


    const result = GridDescriptorNativeFunctions['questpdf__grid_descriptor__item__3e7cf6ba']!(this._ptr, columns);

    return new Container(result);

  }




}


// ============================================================================
// MultiColumnDescriptor - Native Function Declarations
// ============================================================================





const MultiColumnDescriptorNativeFunctions: Record<string, ((...args: any[]) => any) | null> = {};

function initMultiColumnDescriptorFunctions(lib: koffi.IKoffiLib): void {

  MultiColumnDescriptorNativeFunctions['questpdf__multi_column_descriptor__spacing__b96a0ed7'] = lib.func('void questpdf__multi_column_descriptor__spacing__b96a0ed7(void* target, float value, int32_t unit)');

  MultiColumnDescriptorNativeFunctions['questpdf__multi_column_descriptor__columns__f9027e4e'] = lib.func('void questpdf__multi_column_descriptor__columns__f9027e4e(void* target, int32_t value)');

  MultiColumnDescriptorNativeFunctions['questpdf__multi_column_descriptor__balance_height__a0509325'] = lib.func('void questpdf__multi_column_descriptor__balance_height__a0509325(void* target, uint8_t enable)');

  MultiColumnDescriptorNativeFunctions['questpdf__multi_column_descriptor__content__68196264'] = lib.func('void* questpdf__multi_column_descriptor__content__68196264(void* target)');

  MultiColumnDescriptorNativeFunctions['questpdf__multi_column_descriptor__spacer__9d6eea5d'] = lib.func('void* questpdf__multi_column_descriptor__spacer__9d6eea5d(void* target)');



}

// Register initialization with the global initializers
classInitializers.push((lib: koffi.IKoffiLib) => initMultiColumnDescriptorFunctions(lib));

// ============================================================================
// MultiColumnDescriptor Class
// ============================================================================

export class MultiColumnDescriptor {
  private readonly _ptr: NativePointer;
  private readonly _callbacks: koffi.IKoffiRegisteredCallback[] = [];

  constructor(ptr: NativePointer) {
    this._ptr = ptr;
  }

  get pointer(): NativePointer {
    return this._ptr;
  }


  
    spacing(value: number, unit: Unit = Unit.Point): void {


    const result = MultiColumnDescriptorNativeFunctions['questpdf__multi_column_descriptor__spacing__b96a0ed7']!(this._ptr, value, unit);

  }


  
    columns(value: number = 2): void {


    const result = MultiColumnDescriptorNativeFunctions['questpdf__multi_column_descriptor__columns__f9027e4e']!(this._ptr, value);

  }


  
    balanceHeight(enable: boolean = true): void {


    const result = MultiColumnDescriptorNativeFunctions['questpdf__multi_column_descriptor__balance_height__a0509325']!(this._ptr, enable);

  }


  
    content(): Container {


    const result = MultiColumnDescriptorNativeFunctions['questpdf__multi_column_descriptor__content__68196264']!(this._ptr);

    return new Container(result);

  }


  
    spacer(): Container {


    const result = MultiColumnDescriptorNativeFunctions['questpdf__multi_column_descriptor__spacer__9d6eea5d']!(this._ptr);

    return new Container(result);

  }




}


// ============================================================================
// TableCellDescriptor - Native Function Declarations
// ============================================================================





const TableCellDescriptorNativeFunctions: Record<string, ((...args: any[]) => any) | null> = {};

function initTableCellDescriptorFunctions(lib: koffi.IKoffiLib): void {

  TableCellDescriptorNativeFunctions['questpdf__table_cell_descriptor__cell__1061edf9'] = lib.func('void* questpdf__table_cell_descriptor__cell__1061edf9(void* target)');



}

// Register initialization with the global initializers
classInitializers.push((lib: koffi.IKoffiLib) => initTableCellDescriptorFunctions(lib));

// ============================================================================
// TableCellDescriptor Class
// ============================================================================

export class TableCellDescriptor {
  private readonly _ptr: NativePointer;
  private readonly _callbacks: koffi.IKoffiRegisteredCallback[] = [];

  constructor(ptr: NativePointer) {
    this._ptr = ptr;
  }

  get pointer(): NativePointer {
    return this._ptr;
  }


  
    cell(): TableCellContainer {


    const result = TableCellDescriptorNativeFunctions['questpdf__table_cell_descriptor__cell__1061edf9']!(this._ptr);

    return new TableCellContainer(result);

  }




}


// ============================================================================
// TableColumnsDefinitionDescriptor - Native Function Declarations
// ============================================================================





const TableColumnsDefinitionDescriptorNativeFunctions: Record<string, ((...args: any[]) => any) | null> = {};

function initTableColumnsDefinitionDescriptorFunctions(lib: koffi.IKoffiLib): void {

  TableColumnsDefinitionDescriptorNativeFunctions['questpdf__table_columns_definition_descriptor__constant_column__e71e4979'] = lib.func('void questpdf__table_columns_definition_descriptor__constant_column__e71e4979(void* target, float width, int32_t unit)');

  TableColumnsDefinitionDescriptorNativeFunctions['questpdf__table_columns_definition_descriptor__relative_column__940a67b1'] = lib.func('void questpdf__table_columns_definition_descriptor__relative_column__940a67b1(void* target, float width)');



}

// Register initialization with the global initializers
classInitializers.push((lib: koffi.IKoffiLib) => initTableColumnsDefinitionDescriptorFunctions(lib));

// ============================================================================
// TableColumnsDefinitionDescriptor Class
// ============================================================================

export class TableColumnsDefinitionDescriptor {
  private readonly _ptr: NativePointer;
  private readonly _callbacks: koffi.IKoffiRegisteredCallback[] = [];

  constructor(ptr: NativePointer) {
    this._ptr = ptr;
  }

  get pointer(): NativePointer {
    return this._ptr;
  }


  
    constantColumn(width: number, unit: Unit = Unit.Point): void {


    const result = TableColumnsDefinitionDescriptorNativeFunctions['questpdf__table_columns_definition_descriptor__constant_column__e71e4979']!(this._ptr, width, unit);

  }


  
    relativeColumn(width: number = 1): void {


    const result = TableColumnsDefinitionDescriptorNativeFunctions['questpdf__table_columns_definition_descriptor__relative_column__940a67b1']!(this._ptr, width);

  }




}


// ============================================================================
// TableDescriptor - Native Function Declarations
// ============================================================================




// typedef void (*table_columns_definition_descriptor_callback_1b198f41)(void*);

// typedef void (*table_cell_descriptor_callback_227448b3)(void*);

// typedef void (*table_cell_descriptor_callback_a74a23a5)(void*);


const TableDescriptorNativeFunctions: Record<string, ((...args: any[]) => any) | null> = {};

function initTableDescriptorFunctions(lib: koffi.IKoffiLib): void {

  TableDescriptorNativeFunctions['questpdf__table_descriptor__columns_definition__1b198f41'] = lib.func('void questpdf__table_descriptor__columns_definition__1b198f41(void* target, void* handler)');

  TableDescriptorNativeFunctions['questpdf__table_descriptor__extend_last_cells_to_table_bottom__22a2235b'] = lib.func('void questpdf__table_descriptor__extend_last_cells_to_table_bottom__22a2235b(void* target)');

  TableDescriptorNativeFunctions['questpdf__table_descriptor__header__227448b3'] = lib.func('void questpdf__table_descriptor__header__227448b3(void* target, void* handler)');

  TableDescriptorNativeFunctions['questpdf__table_descriptor__footer__a74a23a5'] = lib.func('void questpdf__table_descriptor__footer__a74a23a5(void* target, void* handler)');

  TableDescriptorNativeFunctions['questpdf__table_descriptor__cell__1f40892e'] = lib.func('void* questpdf__table_descriptor__cell__1f40892e(void* target)');



}

// Register initialization with the global initializers
classInitializers.push((lib: koffi.IKoffiLib) => initTableDescriptorFunctions(lib));

// ============================================================================
// TableDescriptor Class
// ============================================================================

export class TableDescriptor {
  private readonly _ptr: NativePointer;
  private readonly _callbacks: koffi.IKoffiRegisteredCallback[] = [];

  constructor(ptr: NativePointer) {
    this._ptr = ptr;
  }

  get pointer(): NativePointer {
    return this._ptr;
  }


  
    columnsDefinition(handler: (arg0: TableColumnsDefinitionDescriptor) => void): void {

    // Create callback type and register handler
    const handlerCallbackType_1b198f41 = koffi.proto('void handlerCb_1b198f41(void* ptr)');
    const handlerCallbackPtr_1b198f41 = koffi.pointer(handlerCallbackType_1b198f41);
    const handlerCb = koffi.register((ptr: NativePointer) => {
      const obj = new TableColumnsDefinitionDescriptor(ptr);
      handler(obj);
    }, handlerCallbackPtr_1b198f41);
    this._callbacks.push(handlerCb);


    const result = TableDescriptorNativeFunctions['questpdf__table_descriptor__columns_definition__1b198f41']!(this._ptr, handlerCb);

  }


  
    extendLastCellsToTableBottom(): void {


    const result = TableDescriptorNativeFunctions['questpdf__table_descriptor__extend_last_cells_to_table_bottom__22a2235b']!(this._ptr);

  }


  
    header(handler: (arg0: TableCellDescriptor) => void): void {

    // Create callback type and register handler
    const handlerCallbackType_227448b3 = koffi.proto('void handlerCb_227448b3(void* ptr)');
    const handlerCallbackPtr_227448b3 = koffi.pointer(handlerCallbackType_227448b3);
    const handlerCb = koffi.register((ptr: NativePointer) => {
      const obj = new TableCellDescriptor(ptr);
      handler(obj);
    }, handlerCallbackPtr_227448b3);
    this._callbacks.push(handlerCb);


    const result = TableDescriptorNativeFunctions['questpdf__table_descriptor__header__227448b3']!(this._ptr, handlerCb);

  }


  
    footer(handler: (arg0: TableCellDescriptor) => void): void {

    // Create callback type and register handler
    const handlerCallbackType_a74a23a5 = koffi.proto('void handlerCb_a74a23a5(void* ptr)');
    const handlerCallbackPtr_a74a23a5 = koffi.pointer(handlerCallbackType_a74a23a5);
    const handlerCb = koffi.register((ptr: NativePointer) => {
      const obj = new TableCellDescriptor(ptr);
      handler(obj);
    }, handlerCallbackPtr_a74a23a5);
    this._callbacks.push(handlerCb);


    const result = TableDescriptorNativeFunctions['questpdf__table_descriptor__footer__a74a23a5']!(this._ptr, handlerCb);

  }


  
    cell(): TableCellContainer {


    const result = TableDescriptorNativeFunctions['questpdf__table_descriptor__cell__1f40892e']!(this._ptr);

    return new TableCellContainer(result);

  }




}


// ============================================================================
// TextDescriptor - Native Function Declarations
// ============================================================================




// typedef void (*container_callback_ff63896d)(void*);


const TextDescriptorNativeFunctions: Record<string, ((...args: any[]) => any) | null> = {};

function initTextDescriptorFunctions(lib: koffi.IKoffiLib): void {

  TextDescriptorNativeFunctions['questpdf__text_descriptor__align_left__4a573634'] = lib.func('void questpdf__text_descriptor__align_left__4a573634(void* target)');

  TextDescriptorNativeFunctions['questpdf__text_descriptor__align_center__def2b616'] = lib.func('void questpdf__text_descriptor__align_center__def2b616(void* target)');

  TextDescriptorNativeFunctions['questpdf__text_descriptor__align_right__de6eaa17'] = lib.func('void questpdf__text_descriptor__align_right__de6eaa17(void* target)');

  TextDescriptorNativeFunctions['questpdf__text_descriptor__justify__1501b0fa'] = lib.func('void questpdf__text_descriptor__justify__1501b0fa(void* target)');

  TextDescriptorNativeFunctions['questpdf__text_descriptor__align_start__947ba696'] = lib.func('void questpdf__text_descriptor__align_start__947ba696(void* target)');

  TextDescriptorNativeFunctions['questpdf__text_descriptor__align_end__5aefafc5'] = lib.func('void questpdf__text_descriptor__align_end__5aefafc5(void* target)');

  TextDescriptorNativeFunctions['questpdf__text_descriptor__clamp_lines__f1b02b03'] = lib.func('void questpdf__text_descriptor__clamp_lines__f1b02b03(void* target, int32_t maxLines, str16 ellipsis)');

  TextDescriptorNativeFunctions['questpdf__text_descriptor__paragraph_spacing__c3629bd6'] = lib.func('void questpdf__text_descriptor__paragraph_spacing__c3629bd6(void* target, float value, int32_t unit)');

  TextDescriptorNativeFunctions['questpdf__text_descriptor__paragraph_first_line_indentation__414498e7'] = lib.func('void questpdf__text_descriptor__paragraph_first_line_indentation__414498e7(void* target, float value, int32_t unit)');

  TextDescriptorNativeFunctions['questpdf__text_descriptor__span__41a383c0'] = lib.func('void* questpdf__text_descriptor__span__41a383c0(void* target, str16 text)');

  TextDescriptorNativeFunctions['questpdf__text_descriptor__line__17db2520'] = lib.func('void* questpdf__text_descriptor__line__17db2520(void* target, str16 text)');

  TextDescriptorNativeFunctions['questpdf__text_descriptor__empty_line__70ae8fc0'] = lib.func('void* questpdf__text_descriptor__empty_line__70ae8fc0(void* target)');

  TextDescriptorNativeFunctions['questpdf__text_descriptor__current_page_number__2097e179'] = lib.func('void* questpdf__text_descriptor__current_page_number__2097e179(void* target)');

  TextDescriptorNativeFunctions['questpdf__text_descriptor__total_pages__604d3e19'] = lib.func('void* questpdf__text_descriptor__total_pages__604d3e19(void* target)');

  TextDescriptorNativeFunctions['questpdf__text_descriptor__begin_page_number_of_section__340accfc'] = lib.func('void* questpdf__text_descriptor__begin_page_number_of_section__340accfc(void* target, str16 sectionName)');

  TextDescriptorNativeFunctions['questpdf__text_descriptor__end_page_number_of_section__deee569a'] = lib.func('void* questpdf__text_descriptor__end_page_number_of_section__deee569a(void* target, str16 sectionName)');

  TextDescriptorNativeFunctions['questpdf__text_descriptor__page_number_within_section__51768233'] = lib.func('void* questpdf__text_descriptor__page_number_within_section__51768233(void* target, str16 sectionName)');

  TextDescriptorNativeFunctions['questpdf__text_descriptor__total_pages_within_section__250c06e5'] = lib.func('void* questpdf__text_descriptor__total_pages_within_section__250c06e5(void* target, str16 sectionName)');

  TextDescriptorNativeFunctions['questpdf__text_descriptor__section_link__c9b32c1a'] = lib.func('void* questpdf__text_descriptor__section_link__c9b32c1a(void* target, str16 text, str16 sectionName)');

  TextDescriptorNativeFunctions['questpdf__text_descriptor__hyperlink__f38a28c7'] = lib.func('void* questpdf__text_descriptor__hyperlink__f38a28c7(void* target, str16 text, str16 url)');

  TextDescriptorNativeFunctions['questpdf__text_descriptor__element__862752ab'] = lib.func('void* questpdf__text_descriptor__element__862752ab(void* target, int32_t alignment)');

  TextDescriptorNativeFunctions['questpdf__text_descriptor__element__ff63896d'] = lib.func('void questpdf__text_descriptor__element__ff63896d(void* target, void* handler, int32_t alignment)');



}

// Register initialization with the global initializers
classInitializers.push((lib: koffi.IKoffiLib) => initTextDescriptorFunctions(lib));

// ============================================================================
// TextDescriptor Class
// ============================================================================

export class TextDescriptor {
  private readonly _ptr: NativePointer;
  private readonly _callbacks: koffi.IKoffiRegisteredCallback[] = [];

  constructor(ptr: NativePointer) {
    this._ptr = ptr;
  }

  get pointer(): NativePointer {
    return this._ptr;
  }


  
    alignLeft(): void {


    const result = TextDescriptorNativeFunctions['questpdf__text_descriptor__align_left__4a573634']!(this._ptr);

  }


  
    alignCenter(): void {


    const result = TextDescriptorNativeFunctions['questpdf__text_descriptor__align_center__def2b616']!(this._ptr);

  }


  
    alignRight(): void {


    const result = TextDescriptorNativeFunctions['questpdf__text_descriptor__align_right__de6eaa17']!(this._ptr);

  }


  
    justify(): void {


    const result = TextDescriptorNativeFunctions['questpdf__text_descriptor__justify__1501b0fa']!(this._ptr);

  }


  
    alignStart(): void {


    const result = TextDescriptorNativeFunctions['questpdf__text_descriptor__align_start__947ba696']!(this._ptr);

  }


  
    alignEnd(): void {


    const result = TextDescriptorNativeFunctions['questpdf__text_descriptor__align_end__5aefafc5']!(this._ptr);

  }


  
    clampLines(maxLines: number, ellipsis: string = '…'): void {


    const result = TextDescriptorNativeFunctions['questpdf__text_descriptor__clamp_lines__f1b02b03']!(this._ptr, maxLines, ellipsis);

  }


  
    paragraphSpacing(value: number, unit: Unit = Unit.Point): void {


    const result = TextDescriptorNativeFunctions['questpdf__text_descriptor__paragraph_spacing__c3629bd6']!(this._ptr, value, unit);

  }


  
    paragraphFirstLineIndentation(value: number, unit: Unit = Unit.Point): void {


    const result = TextDescriptorNativeFunctions['questpdf__text_descriptor__paragraph_first_line_indentation__414498e7']!(this._ptr, value, unit);

  }


  
    span(text: string): TextSpanDescriptor {


    const result = TextDescriptorNativeFunctions['questpdf__text_descriptor__span__41a383c0']!(this._ptr, text);

    return new TextSpanDescriptor(result);

  }


  
    line(text: string): TextSpanDescriptor {


    const result = TextDescriptorNativeFunctions['questpdf__text_descriptor__line__17db2520']!(this._ptr, text);

    return new TextSpanDescriptor(result);

  }


  
    emptyLine(): TextSpanDescriptor {


    const result = TextDescriptorNativeFunctions['questpdf__text_descriptor__empty_line__70ae8fc0']!(this._ptr);

    return new TextSpanDescriptor(result);

  }


  
    currentPageNumber(): TextPageNumberDescriptor {


    const result = TextDescriptorNativeFunctions['questpdf__text_descriptor__current_page_number__2097e179']!(this._ptr);

    return new TextPageNumberDescriptor(result);

  }


  
    totalPages(): TextPageNumberDescriptor {


    const result = TextDescriptorNativeFunctions['questpdf__text_descriptor__total_pages__604d3e19']!(this._ptr);

    return new TextPageNumberDescriptor(result);

  }


  
    beginPageNumberOfSection(sectionName: string): TextPageNumberDescriptor {


    const result = TextDescriptorNativeFunctions['questpdf__text_descriptor__begin_page_number_of_section__340accfc']!(this._ptr, sectionName);

    return new TextPageNumberDescriptor(result);

  }


  
    endPageNumberOfSection(sectionName: string): TextPageNumberDescriptor {


    const result = TextDescriptorNativeFunctions['questpdf__text_descriptor__end_page_number_of_section__deee569a']!(this._ptr, sectionName);

    return new TextPageNumberDescriptor(result);

  }


  
    pageNumberWithinSection(sectionName: string): TextPageNumberDescriptor {


    const result = TextDescriptorNativeFunctions['questpdf__text_descriptor__page_number_within_section__51768233']!(this._ptr, sectionName);

    return new TextPageNumberDescriptor(result);

  }


  
    totalPagesWithinSection(sectionName: string): TextPageNumberDescriptor {


    const result = TextDescriptorNativeFunctions['questpdf__text_descriptor__total_pages_within_section__250c06e5']!(this._ptr, sectionName);

    return new TextPageNumberDescriptor(result);

  }


  
    sectionLink(text: string, sectionName: string): TextSpanDescriptor {


    const result = TextDescriptorNativeFunctions['questpdf__text_descriptor__section_link__c9b32c1a']!(this._ptr, text, sectionName);

    return new TextSpanDescriptor(result);

  }


  
    hyperlink(text: string, url: string): TextSpanDescriptor {


    const result = TextDescriptorNativeFunctions['questpdf__text_descriptor__hyperlink__f38a28c7']!(this._ptr, text, url);

    return new TextSpanDescriptor(result);

  }


  
    private element_TextInjectedElementAlignment(alignment: TextInjectedElementAlignment = TextInjectedElementAlignment.AboveBaseline): Container {


    const result = TextDescriptorNativeFunctions['questpdf__text_descriptor__element__862752ab']!(this._ptr, alignment);

    return new Container(result);

  }


  
    private element_Action_TextInjectedElementAlignment(handler: (arg0: Container) => void, alignment: TextInjectedElementAlignment = TextInjectedElementAlignment.AboveBaseline): void {

    // Create callback type and register handler
    const handlerCallbackType_ff63896d = koffi.proto('void handlerCb_ff63896d(void* ptr)');
    const handlerCallbackPtr_ff63896d = koffi.pointer(handlerCallbackType_ff63896d);
    const handlerCb = koffi.register((ptr: NativePointer) => {
      const obj = new Container(ptr);
      handler(obj);
    }, handlerCallbackPtr_ff63896d);
    this._callbacks.push(handlerCb);


    const result = TextDescriptorNativeFunctions['questpdf__text_descriptor__element__ff63896d']!(this._ptr, handlerCb, alignment);

  }




}


// ============================================================================
// TextSpanDescriptor - Native Function Declarations
// ============================================================================





const TextSpanDescriptorNativeFunctions: Record<string, ((...args: any[]) => any) | null> = {};

function initTextSpanDescriptorFunctions(lib: koffi.IKoffiLib): void {

  TextSpanDescriptorNativeFunctions['questpdf__text_span_descriptor__font_color__a0d06e42'] = lib.func('void* questpdf__text_span_descriptor__font_color__a0d06e42(void* target, uint32_t color)');

  TextSpanDescriptorNativeFunctions['questpdf__text_span_descriptor__background_color__5461b453'] = lib.func('void* questpdf__text_span_descriptor__background_color__5461b453(void* target, uint32_t color)');

  TextSpanDescriptorNativeFunctions['questpdf__text_span_descriptor__font_size__c989487d'] = lib.func('void* questpdf__text_span_descriptor__font_size__c989487d(void* target, float value)');

  TextSpanDescriptorNativeFunctions['questpdf__text_span_descriptor__line_height__a1b4697a'] = lib.func('void* questpdf__text_span_descriptor__line_height__a1b4697a(void* target, void* factor)');

  TextSpanDescriptorNativeFunctions['questpdf__text_span_descriptor__letter_spacing__92f86a26'] = lib.func('void* questpdf__text_span_descriptor__letter_spacing__92f86a26(void* target, float factor)');

  TextSpanDescriptorNativeFunctions['questpdf__text_span_descriptor__word_spacing__1f794add'] = lib.func('void* questpdf__text_span_descriptor__word_spacing__1f794add(void* target, float factor)');

  TextSpanDescriptorNativeFunctions['questpdf__text_span_descriptor__italic__4f023aba'] = lib.func('void* questpdf__text_span_descriptor__italic__4f023aba(void* target, uint8_t value)');

  TextSpanDescriptorNativeFunctions['questpdf__text_span_descriptor__strikethrough__41841206'] = lib.func('void* questpdf__text_span_descriptor__strikethrough__41841206(void* target, uint8_t value)');

  TextSpanDescriptorNativeFunctions['questpdf__text_span_descriptor__underline__2e1ae473'] = lib.func('void* questpdf__text_span_descriptor__underline__2e1ae473(void* target, uint8_t value)');

  TextSpanDescriptorNativeFunctions['questpdf__text_span_descriptor__overline__add25860'] = lib.func('void* questpdf__text_span_descriptor__overline__add25860(void* target, uint8_t value)');

  TextSpanDescriptorNativeFunctions['questpdf__text_span_descriptor__decoration_color__5d18d151'] = lib.func('void* questpdf__text_span_descriptor__decoration_color__5d18d151(void* target, uint32_t color)');

  TextSpanDescriptorNativeFunctions['questpdf__text_span_descriptor__decoration_thickness__c7c23c84'] = lib.func('void* questpdf__text_span_descriptor__decoration_thickness__c7c23c84(void* target, float factor)');

  TextSpanDescriptorNativeFunctions['questpdf__text_span_descriptor__decoration_solid__f64746d1'] = lib.func('void* questpdf__text_span_descriptor__decoration_solid__f64746d1(void* target)');

  TextSpanDescriptorNativeFunctions['questpdf__text_span_descriptor__decoration_double__41cf8a18'] = lib.func('void* questpdf__text_span_descriptor__decoration_double__41cf8a18(void* target)');

  TextSpanDescriptorNativeFunctions['questpdf__text_span_descriptor__decoration_wavy__1761acf2'] = lib.func('void* questpdf__text_span_descriptor__decoration_wavy__1761acf2(void* target)');

  TextSpanDescriptorNativeFunctions['questpdf__text_span_descriptor__decoration_dotted__e940537a'] = lib.func('void* questpdf__text_span_descriptor__decoration_dotted__e940537a(void* target)');

  TextSpanDescriptorNativeFunctions['questpdf__text_span_descriptor__decoration_dashed__a85f7344'] = lib.func('void* questpdf__text_span_descriptor__decoration_dashed__a85f7344(void* target)');

  TextSpanDescriptorNativeFunctions['questpdf__text_span_descriptor__thin__e9036638'] = lib.func('void* questpdf__text_span_descriptor__thin__e9036638(void* target)');

  TextSpanDescriptorNativeFunctions['questpdf__text_span_descriptor__extra_light__33bbe020'] = lib.func('void* questpdf__text_span_descriptor__extra_light__33bbe020(void* target)');

  TextSpanDescriptorNativeFunctions['questpdf__text_span_descriptor__light__37ef1bc2'] = lib.func('void* questpdf__text_span_descriptor__light__37ef1bc2(void* target)');

  TextSpanDescriptorNativeFunctions['questpdf__text_span_descriptor__normal_weight__18d360b3'] = lib.func('void* questpdf__text_span_descriptor__normal_weight__18d360b3(void* target)');

  TextSpanDescriptorNativeFunctions['questpdf__text_span_descriptor__medium__5ef8b80e'] = lib.func('void* questpdf__text_span_descriptor__medium__5ef8b80e(void* target)');

  TextSpanDescriptorNativeFunctions['questpdf__text_span_descriptor__semi_bold__0b92f7b7'] = lib.func('void* questpdf__text_span_descriptor__semi_bold__0b92f7b7(void* target)');

  TextSpanDescriptorNativeFunctions['questpdf__text_span_descriptor__bold__0dfa9061'] = lib.func('void* questpdf__text_span_descriptor__bold__0dfa9061(void* target)');

  TextSpanDescriptorNativeFunctions['questpdf__text_span_descriptor__extra_bold__c4fbc0a6'] = lib.func('void* questpdf__text_span_descriptor__extra_bold__c4fbc0a6(void* target)');

  TextSpanDescriptorNativeFunctions['questpdf__text_span_descriptor__black__0cc8d698'] = lib.func('void* questpdf__text_span_descriptor__black__0cc8d698(void* target)');

  TextSpanDescriptorNativeFunctions['questpdf__text_span_descriptor__extra_black__c7698d85'] = lib.func('void* questpdf__text_span_descriptor__extra_black__c7698d85(void* target)');

  TextSpanDescriptorNativeFunctions['questpdf__text_span_descriptor__normal_position__5e5176cb'] = lib.func('void* questpdf__text_span_descriptor__normal_position__5e5176cb(void* target)');

  TextSpanDescriptorNativeFunctions['questpdf__text_span_descriptor__subscript__db9bd4eb'] = lib.func('void* questpdf__text_span_descriptor__subscript__db9bd4eb(void* target)');

  TextSpanDescriptorNativeFunctions['questpdf__text_span_descriptor__superscript__a9b46b1e'] = lib.func('void* questpdf__text_span_descriptor__superscript__a9b46b1e(void* target)');

  TextSpanDescriptorNativeFunctions['questpdf__text_span_descriptor__direction_auto__fbed9e71'] = lib.func('void* questpdf__text_span_descriptor__direction_auto__fbed9e71(void* target)');

  TextSpanDescriptorNativeFunctions['questpdf__text_span_descriptor__direction_from_left_to_right__09e2e3bc'] = lib.func('void* questpdf__text_span_descriptor__direction_from_left_to_right__09e2e3bc(void* target)');

  TextSpanDescriptorNativeFunctions['questpdf__text_span_descriptor__direction_from_right_to_left__6cc5cb9e'] = lib.func('void* questpdf__text_span_descriptor__direction_from_right_to_left__6cc5cb9e(void* target)');

  TextSpanDescriptorNativeFunctions['questpdf__text_span_descriptor__enable_font_feature__136a164d'] = lib.func('void* questpdf__text_span_descriptor__enable_font_feature__136a164d(void* target, str16 featureName)');

  TextSpanDescriptorNativeFunctions['questpdf__text_span_descriptor__disable_font_feature__5bd81de9'] = lib.func('void* questpdf__text_span_descriptor__disable_font_feature__5bd81de9(void* target, str16 featureName)');



}

// Register initialization with the global initializers
classInitializers.push((lib: koffi.IKoffiLib) => initTextSpanDescriptorFunctions(lib));

// ============================================================================
// TextSpanDescriptor Class
// ============================================================================

export class TextSpanDescriptor {
  private readonly _ptr: NativePointer;
  private readonly _callbacks: koffi.IKoffiRegisteredCallback[] = [];

  constructor(ptr: NativePointer) {
    this._ptr = ptr;
  }

  get pointer(): NativePointer {
    return this._ptr;
  }


  
    fontColor(color: Color): TextSpanDescriptor {


    const result = TextSpanDescriptorNativeFunctions['questpdf__text_span_descriptor__font_color__a0d06e42']!(this._ptr, color.hex);

    return new TextSpanDescriptor(result);

  }


  
    backgroundColor(color: Color): TextSpanDescriptor {


    const result = TextSpanDescriptorNativeFunctions['questpdf__text_span_descriptor__background_color__5461b453']!(this._ptr, color.hex);

    return new TextSpanDescriptor(result);

  }


  
    fontSize(value: number): TextSpanDescriptor {


    const result = TextSpanDescriptorNativeFunctions['questpdf__text_span_descriptor__font_size__c989487d']!(this._ptr, value);

    return new TextSpanDescriptor(result);

  }


  
    lineHeight(factor: unknown): TextSpanDescriptor {


    const result = TextSpanDescriptorNativeFunctions['questpdf__text_span_descriptor__line_height__a1b4697a']!(this._ptr, factor);

    return new TextSpanDescriptor(result);

  }


  
    letterSpacing(factor: number = 0): TextSpanDescriptor {


    const result = TextSpanDescriptorNativeFunctions['questpdf__text_span_descriptor__letter_spacing__92f86a26']!(this._ptr, factor);

    return new TextSpanDescriptor(result);

  }


  
    wordSpacing(factor: number = 0): TextSpanDescriptor {


    const result = TextSpanDescriptorNativeFunctions['questpdf__text_span_descriptor__word_spacing__1f794add']!(this._ptr, factor);

    return new TextSpanDescriptor(result);

  }


  
    italic(value: boolean = true): TextSpanDescriptor {


    const result = TextSpanDescriptorNativeFunctions['questpdf__text_span_descriptor__italic__4f023aba']!(this._ptr, value);

    return new TextSpanDescriptor(result);

  }


  
    strikethrough(value: boolean = true): TextSpanDescriptor {


    const result = TextSpanDescriptorNativeFunctions['questpdf__text_span_descriptor__strikethrough__41841206']!(this._ptr, value);

    return new TextSpanDescriptor(result);

  }


  
    underline(value: boolean = true): TextSpanDescriptor {


    const result = TextSpanDescriptorNativeFunctions['questpdf__text_span_descriptor__underline__2e1ae473']!(this._ptr, value);

    return new TextSpanDescriptor(result);

  }


  
    overline(value: boolean = true): TextSpanDescriptor {


    const result = TextSpanDescriptorNativeFunctions['questpdf__text_span_descriptor__overline__add25860']!(this._ptr, value);

    return new TextSpanDescriptor(result);

  }


  
    decorationColor(color: Color): TextSpanDescriptor {


    const result = TextSpanDescriptorNativeFunctions['questpdf__text_span_descriptor__decoration_color__5d18d151']!(this._ptr, color.hex);

    return new TextSpanDescriptor(result);

  }


  
    decorationThickness(factor: number): TextSpanDescriptor {


    const result = TextSpanDescriptorNativeFunctions['questpdf__text_span_descriptor__decoration_thickness__c7c23c84']!(this._ptr, factor);

    return new TextSpanDescriptor(result);

  }


  
    decorationSolid(): TextSpanDescriptor {


    const result = TextSpanDescriptorNativeFunctions['questpdf__text_span_descriptor__decoration_solid__f64746d1']!(this._ptr);

    return new TextSpanDescriptor(result);

  }


  
    decorationDouble(): TextSpanDescriptor {


    const result = TextSpanDescriptorNativeFunctions['questpdf__text_span_descriptor__decoration_double__41cf8a18']!(this._ptr);

    return new TextSpanDescriptor(result);

  }


  
    decorationWavy(): TextSpanDescriptor {


    const result = TextSpanDescriptorNativeFunctions['questpdf__text_span_descriptor__decoration_wavy__1761acf2']!(this._ptr);

    return new TextSpanDescriptor(result);

  }


  
    decorationDotted(): TextSpanDescriptor {


    const result = TextSpanDescriptorNativeFunctions['questpdf__text_span_descriptor__decoration_dotted__e940537a']!(this._ptr);

    return new TextSpanDescriptor(result);

  }


  
    decorationDashed(): TextSpanDescriptor {


    const result = TextSpanDescriptorNativeFunctions['questpdf__text_span_descriptor__decoration_dashed__a85f7344']!(this._ptr);

    return new TextSpanDescriptor(result);

  }


  
    thin(): TextSpanDescriptor {


    const result = TextSpanDescriptorNativeFunctions['questpdf__text_span_descriptor__thin__e9036638']!(this._ptr);

    return new TextSpanDescriptor(result);

  }


  
    extraLight(): TextSpanDescriptor {


    const result = TextSpanDescriptorNativeFunctions['questpdf__text_span_descriptor__extra_light__33bbe020']!(this._ptr);

    return new TextSpanDescriptor(result);

  }


  
    light(): TextSpanDescriptor {


    const result = TextSpanDescriptorNativeFunctions['questpdf__text_span_descriptor__light__37ef1bc2']!(this._ptr);

    return new TextSpanDescriptor(result);

  }


  
    normalWeight(): TextSpanDescriptor {


    const result = TextSpanDescriptorNativeFunctions['questpdf__text_span_descriptor__normal_weight__18d360b3']!(this._ptr);

    return new TextSpanDescriptor(result);

  }


  
    medium(): TextSpanDescriptor {


    const result = TextSpanDescriptorNativeFunctions['questpdf__text_span_descriptor__medium__5ef8b80e']!(this._ptr);

    return new TextSpanDescriptor(result);

  }


  
    semiBold(): TextSpanDescriptor {


    const result = TextSpanDescriptorNativeFunctions['questpdf__text_span_descriptor__semi_bold__0b92f7b7']!(this._ptr);

    return new TextSpanDescriptor(result);

  }


  
    bold(): TextSpanDescriptor {


    const result = TextSpanDescriptorNativeFunctions['questpdf__text_span_descriptor__bold__0dfa9061']!(this._ptr);

    return new TextSpanDescriptor(result);

  }


  
    extraBold(): TextSpanDescriptor {


    const result = TextSpanDescriptorNativeFunctions['questpdf__text_span_descriptor__extra_bold__c4fbc0a6']!(this._ptr);

    return new TextSpanDescriptor(result);

  }


  
    black(): TextSpanDescriptor {


    const result = TextSpanDescriptorNativeFunctions['questpdf__text_span_descriptor__black__0cc8d698']!(this._ptr);

    return new TextSpanDescriptor(result);

  }


  
    extraBlack(): TextSpanDescriptor {


    const result = TextSpanDescriptorNativeFunctions['questpdf__text_span_descriptor__extra_black__c7698d85']!(this._ptr);

    return new TextSpanDescriptor(result);

  }


  
    normalPosition(): TextSpanDescriptor {


    const result = TextSpanDescriptorNativeFunctions['questpdf__text_span_descriptor__normal_position__5e5176cb']!(this._ptr);

    return new TextSpanDescriptor(result);

  }


  
    subscript(): TextSpanDescriptor {


    const result = TextSpanDescriptorNativeFunctions['questpdf__text_span_descriptor__subscript__db9bd4eb']!(this._ptr);

    return new TextSpanDescriptor(result);

  }


  
    superscript(): TextSpanDescriptor {


    const result = TextSpanDescriptorNativeFunctions['questpdf__text_span_descriptor__superscript__a9b46b1e']!(this._ptr);

    return new TextSpanDescriptor(result);

  }


  
    directionAuto(): TextSpanDescriptor {


    const result = TextSpanDescriptorNativeFunctions['questpdf__text_span_descriptor__direction_auto__fbed9e71']!(this._ptr);

    return new TextSpanDescriptor(result);

  }


  
    directionFromLeftToRight(): TextSpanDescriptor {


    const result = TextSpanDescriptorNativeFunctions['questpdf__text_span_descriptor__direction_from_left_to_right__09e2e3bc']!(this._ptr);

    return new TextSpanDescriptor(result);

  }


  
    directionFromRightToLeft(): TextSpanDescriptor {


    const result = TextSpanDescriptorNativeFunctions['questpdf__text_span_descriptor__direction_from_right_to_left__6cc5cb9e']!(this._ptr);

    return new TextSpanDescriptor(result);

  }


  
    enableFontFeature(featureName: string): TextSpanDescriptor {


    const result = TextSpanDescriptorNativeFunctions['questpdf__text_span_descriptor__enable_font_feature__136a164d']!(this._ptr, featureName);

    return new TextSpanDescriptor(result);

  }


  
    disableFontFeature(featureName: string): TextSpanDescriptor {


    const result = TextSpanDescriptorNativeFunctions['questpdf__text_span_descriptor__disable_font_feature__5bd81de9']!(this._ptr, featureName);

    return new TextSpanDescriptor(result);

  }




}


// ============================================================================
// TextPageNumberDescriptor - Native Function Declarations
// ============================================================================





const TextPageNumberDescriptorNativeFunctions: Record<string, ((...args: any[]) => any) | null> = {};

function initTextPageNumberDescriptorFunctions(lib: koffi.IKoffiLib): void {

  TextPageNumberDescriptorNativeFunctions['questpdf__text_page_number_descriptor__font_color__a0d06e42'] = lib.func('void* questpdf__text_page_number_descriptor__font_color__a0d06e42(void* target, uint32_t color)');

  TextPageNumberDescriptorNativeFunctions['questpdf__text_page_number_descriptor__background_color__5461b453'] = lib.func('void* questpdf__text_page_number_descriptor__background_color__5461b453(void* target, uint32_t color)');

  TextPageNumberDescriptorNativeFunctions['questpdf__text_page_number_descriptor__font_size__c989487d'] = lib.func('void* questpdf__text_page_number_descriptor__font_size__c989487d(void* target, float value)');

  TextPageNumberDescriptorNativeFunctions['questpdf__text_page_number_descriptor__line_height__a1b4697a'] = lib.func('void* questpdf__text_page_number_descriptor__line_height__a1b4697a(void* target, void* factor)');

  TextPageNumberDescriptorNativeFunctions['questpdf__text_page_number_descriptor__letter_spacing__92f86a26'] = lib.func('void* questpdf__text_page_number_descriptor__letter_spacing__92f86a26(void* target, float factor)');

  TextPageNumberDescriptorNativeFunctions['questpdf__text_page_number_descriptor__word_spacing__1f794add'] = lib.func('void* questpdf__text_page_number_descriptor__word_spacing__1f794add(void* target, float factor)');

  TextPageNumberDescriptorNativeFunctions['questpdf__text_page_number_descriptor__italic__4f023aba'] = lib.func('void* questpdf__text_page_number_descriptor__italic__4f023aba(void* target, uint8_t value)');

  TextPageNumberDescriptorNativeFunctions['questpdf__text_page_number_descriptor__strikethrough__41841206'] = lib.func('void* questpdf__text_page_number_descriptor__strikethrough__41841206(void* target, uint8_t value)');

  TextPageNumberDescriptorNativeFunctions['questpdf__text_page_number_descriptor__underline__2e1ae473'] = lib.func('void* questpdf__text_page_number_descriptor__underline__2e1ae473(void* target, uint8_t value)');

  TextPageNumberDescriptorNativeFunctions['questpdf__text_page_number_descriptor__overline__add25860'] = lib.func('void* questpdf__text_page_number_descriptor__overline__add25860(void* target, uint8_t value)');

  TextPageNumberDescriptorNativeFunctions['questpdf__text_page_number_descriptor__decoration_color__5d18d151'] = lib.func('void* questpdf__text_page_number_descriptor__decoration_color__5d18d151(void* target, uint32_t color)');

  TextPageNumberDescriptorNativeFunctions['questpdf__text_page_number_descriptor__decoration_thickness__c7c23c84'] = lib.func('void* questpdf__text_page_number_descriptor__decoration_thickness__c7c23c84(void* target, float factor)');

  TextPageNumberDescriptorNativeFunctions['questpdf__text_page_number_descriptor__decoration_solid__f64746d1'] = lib.func('void* questpdf__text_page_number_descriptor__decoration_solid__f64746d1(void* target)');

  TextPageNumberDescriptorNativeFunctions['questpdf__text_page_number_descriptor__decoration_double__41cf8a18'] = lib.func('void* questpdf__text_page_number_descriptor__decoration_double__41cf8a18(void* target)');

  TextPageNumberDescriptorNativeFunctions['questpdf__text_page_number_descriptor__decoration_wavy__1761acf2'] = lib.func('void* questpdf__text_page_number_descriptor__decoration_wavy__1761acf2(void* target)');

  TextPageNumberDescriptorNativeFunctions['questpdf__text_page_number_descriptor__decoration_dotted__e940537a'] = lib.func('void* questpdf__text_page_number_descriptor__decoration_dotted__e940537a(void* target)');

  TextPageNumberDescriptorNativeFunctions['questpdf__text_page_number_descriptor__decoration_dashed__a85f7344'] = lib.func('void* questpdf__text_page_number_descriptor__decoration_dashed__a85f7344(void* target)');

  TextPageNumberDescriptorNativeFunctions['questpdf__text_page_number_descriptor__thin__e9036638'] = lib.func('void* questpdf__text_page_number_descriptor__thin__e9036638(void* target)');

  TextPageNumberDescriptorNativeFunctions['questpdf__text_page_number_descriptor__extra_light__33bbe020'] = lib.func('void* questpdf__text_page_number_descriptor__extra_light__33bbe020(void* target)');

  TextPageNumberDescriptorNativeFunctions['questpdf__text_page_number_descriptor__light__37ef1bc2'] = lib.func('void* questpdf__text_page_number_descriptor__light__37ef1bc2(void* target)');

  TextPageNumberDescriptorNativeFunctions['questpdf__text_page_number_descriptor__normal_weight__18d360b3'] = lib.func('void* questpdf__text_page_number_descriptor__normal_weight__18d360b3(void* target)');

  TextPageNumberDescriptorNativeFunctions['questpdf__text_page_number_descriptor__medium__5ef8b80e'] = lib.func('void* questpdf__text_page_number_descriptor__medium__5ef8b80e(void* target)');

  TextPageNumberDescriptorNativeFunctions['questpdf__text_page_number_descriptor__semi_bold__0b92f7b7'] = lib.func('void* questpdf__text_page_number_descriptor__semi_bold__0b92f7b7(void* target)');

  TextPageNumberDescriptorNativeFunctions['questpdf__text_page_number_descriptor__bold__0dfa9061'] = lib.func('void* questpdf__text_page_number_descriptor__bold__0dfa9061(void* target)');

  TextPageNumberDescriptorNativeFunctions['questpdf__text_page_number_descriptor__extra_bold__c4fbc0a6'] = lib.func('void* questpdf__text_page_number_descriptor__extra_bold__c4fbc0a6(void* target)');

  TextPageNumberDescriptorNativeFunctions['questpdf__text_page_number_descriptor__black__0cc8d698'] = lib.func('void* questpdf__text_page_number_descriptor__black__0cc8d698(void* target)');

  TextPageNumberDescriptorNativeFunctions['questpdf__text_page_number_descriptor__extra_black__c7698d85'] = lib.func('void* questpdf__text_page_number_descriptor__extra_black__c7698d85(void* target)');

  TextPageNumberDescriptorNativeFunctions['questpdf__text_page_number_descriptor__normal_position__5e5176cb'] = lib.func('void* questpdf__text_page_number_descriptor__normal_position__5e5176cb(void* target)');

  TextPageNumberDescriptorNativeFunctions['questpdf__text_page_number_descriptor__subscript__db9bd4eb'] = lib.func('void* questpdf__text_page_number_descriptor__subscript__db9bd4eb(void* target)');

  TextPageNumberDescriptorNativeFunctions['questpdf__text_page_number_descriptor__superscript__a9b46b1e'] = lib.func('void* questpdf__text_page_number_descriptor__superscript__a9b46b1e(void* target)');

  TextPageNumberDescriptorNativeFunctions['questpdf__text_page_number_descriptor__direction_auto__fbed9e71'] = lib.func('void* questpdf__text_page_number_descriptor__direction_auto__fbed9e71(void* target)');

  TextPageNumberDescriptorNativeFunctions['questpdf__text_page_number_descriptor__direction_from_left_to_right__09e2e3bc'] = lib.func('void* questpdf__text_page_number_descriptor__direction_from_left_to_right__09e2e3bc(void* target)');

  TextPageNumberDescriptorNativeFunctions['questpdf__text_page_number_descriptor__direction_from_right_to_left__6cc5cb9e'] = lib.func('void* questpdf__text_page_number_descriptor__direction_from_right_to_left__6cc5cb9e(void* target)');

  TextPageNumberDescriptorNativeFunctions['questpdf__text_page_number_descriptor__enable_font_feature__136a164d'] = lib.func('void* questpdf__text_page_number_descriptor__enable_font_feature__136a164d(void* target, str16 featureName)');

  TextPageNumberDescriptorNativeFunctions['questpdf__text_page_number_descriptor__disable_font_feature__5bd81de9'] = lib.func('void* questpdf__text_page_number_descriptor__disable_font_feature__5bd81de9(void* target, str16 featureName)');



}

// Register initialization with the global initializers
classInitializers.push((lib: koffi.IKoffiLib) => initTextPageNumberDescriptorFunctions(lib));

// ============================================================================
// TextPageNumberDescriptor Class
// ============================================================================

export class TextPageNumberDescriptor {
  private readonly _ptr: NativePointer;
  private readonly _callbacks: koffi.IKoffiRegisteredCallback[] = [];

  constructor(ptr: NativePointer) {
    this._ptr = ptr;
  }

  get pointer(): NativePointer {
    return this._ptr;
  }


  
    fontColor(color: Color): TextPageNumberDescriptor {


    const result = TextPageNumberDescriptorNativeFunctions['questpdf__text_page_number_descriptor__font_color__a0d06e42']!(this._ptr, color.hex);

    return new TextPageNumberDescriptor(result);

  }


  
    backgroundColor(color: Color): TextPageNumberDescriptor {


    const result = TextPageNumberDescriptorNativeFunctions['questpdf__text_page_number_descriptor__background_color__5461b453']!(this._ptr, color.hex);

    return new TextPageNumberDescriptor(result);

  }


  
    fontSize(value: number): TextPageNumberDescriptor {


    const result = TextPageNumberDescriptorNativeFunctions['questpdf__text_page_number_descriptor__font_size__c989487d']!(this._ptr, value);

    return new TextPageNumberDescriptor(result);

  }


  
    lineHeight(factor: unknown): TextPageNumberDescriptor {


    const result = TextPageNumberDescriptorNativeFunctions['questpdf__text_page_number_descriptor__line_height__a1b4697a']!(this._ptr, factor);

    return new TextPageNumberDescriptor(result);

  }


  
    letterSpacing(factor: number = 0): TextPageNumberDescriptor {


    const result = TextPageNumberDescriptorNativeFunctions['questpdf__text_page_number_descriptor__letter_spacing__92f86a26']!(this._ptr, factor);

    return new TextPageNumberDescriptor(result);

  }


  
    wordSpacing(factor: number = 0): TextPageNumberDescriptor {


    const result = TextPageNumberDescriptorNativeFunctions['questpdf__text_page_number_descriptor__word_spacing__1f794add']!(this._ptr, factor);

    return new TextPageNumberDescriptor(result);

  }


  
    italic(value: boolean = true): TextPageNumberDescriptor {


    const result = TextPageNumberDescriptorNativeFunctions['questpdf__text_page_number_descriptor__italic__4f023aba']!(this._ptr, value);

    return new TextPageNumberDescriptor(result);

  }


  
    strikethrough(value: boolean = true): TextPageNumberDescriptor {


    const result = TextPageNumberDescriptorNativeFunctions['questpdf__text_page_number_descriptor__strikethrough__41841206']!(this._ptr, value);

    return new TextPageNumberDescriptor(result);

  }


  
    underline(value: boolean = true): TextPageNumberDescriptor {


    const result = TextPageNumberDescriptorNativeFunctions['questpdf__text_page_number_descriptor__underline__2e1ae473']!(this._ptr, value);

    return new TextPageNumberDescriptor(result);

  }


  
    overline(value: boolean = true): TextPageNumberDescriptor {


    const result = TextPageNumberDescriptorNativeFunctions['questpdf__text_page_number_descriptor__overline__add25860']!(this._ptr, value);

    return new TextPageNumberDescriptor(result);

  }


  
    decorationColor(color: Color): TextPageNumberDescriptor {


    const result = TextPageNumberDescriptorNativeFunctions['questpdf__text_page_number_descriptor__decoration_color__5d18d151']!(this._ptr, color.hex);

    return new TextPageNumberDescriptor(result);

  }


  
    decorationThickness(factor: number): TextPageNumberDescriptor {


    const result = TextPageNumberDescriptorNativeFunctions['questpdf__text_page_number_descriptor__decoration_thickness__c7c23c84']!(this._ptr, factor);

    return new TextPageNumberDescriptor(result);

  }


  
    decorationSolid(): TextPageNumberDescriptor {


    const result = TextPageNumberDescriptorNativeFunctions['questpdf__text_page_number_descriptor__decoration_solid__f64746d1']!(this._ptr);

    return new TextPageNumberDescriptor(result);

  }


  
    decorationDouble(): TextPageNumberDescriptor {


    const result = TextPageNumberDescriptorNativeFunctions['questpdf__text_page_number_descriptor__decoration_double__41cf8a18']!(this._ptr);

    return new TextPageNumberDescriptor(result);

  }


  
    decorationWavy(): TextPageNumberDescriptor {


    const result = TextPageNumberDescriptorNativeFunctions['questpdf__text_page_number_descriptor__decoration_wavy__1761acf2']!(this._ptr);

    return new TextPageNumberDescriptor(result);

  }


  
    decorationDotted(): TextPageNumberDescriptor {


    const result = TextPageNumberDescriptorNativeFunctions['questpdf__text_page_number_descriptor__decoration_dotted__e940537a']!(this._ptr);

    return new TextPageNumberDescriptor(result);

  }


  
    decorationDashed(): TextPageNumberDescriptor {


    const result = TextPageNumberDescriptorNativeFunctions['questpdf__text_page_number_descriptor__decoration_dashed__a85f7344']!(this._ptr);

    return new TextPageNumberDescriptor(result);

  }


  
    thin(): TextPageNumberDescriptor {


    const result = TextPageNumberDescriptorNativeFunctions['questpdf__text_page_number_descriptor__thin__e9036638']!(this._ptr);

    return new TextPageNumberDescriptor(result);

  }


  
    extraLight(): TextPageNumberDescriptor {


    const result = TextPageNumberDescriptorNativeFunctions['questpdf__text_page_number_descriptor__extra_light__33bbe020']!(this._ptr);

    return new TextPageNumberDescriptor(result);

  }


  
    light(): TextPageNumberDescriptor {


    const result = TextPageNumberDescriptorNativeFunctions['questpdf__text_page_number_descriptor__light__37ef1bc2']!(this._ptr);

    return new TextPageNumberDescriptor(result);

  }


  
    normalWeight(): TextPageNumberDescriptor {


    const result = TextPageNumberDescriptorNativeFunctions['questpdf__text_page_number_descriptor__normal_weight__18d360b3']!(this._ptr);

    return new TextPageNumberDescriptor(result);

  }


  
    medium(): TextPageNumberDescriptor {


    const result = TextPageNumberDescriptorNativeFunctions['questpdf__text_page_number_descriptor__medium__5ef8b80e']!(this._ptr);

    return new TextPageNumberDescriptor(result);

  }


  
    semiBold(): TextPageNumberDescriptor {


    const result = TextPageNumberDescriptorNativeFunctions['questpdf__text_page_number_descriptor__semi_bold__0b92f7b7']!(this._ptr);

    return new TextPageNumberDescriptor(result);

  }


  
    bold(): TextPageNumberDescriptor {


    const result = TextPageNumberDescriptorNativeFunctions['questpdf__text_page_number_descriptor__bold__0dfa9061']!(this._ptr);

    return new TextPageNumberDescriptor(result);

  }


  
    extraBold(): TextPageNumberDescriptor {


    const result = TextPageNumberDescriptorNativeFunctions['questpdf__text_page_number_descriptor__extra_bold__c4fbc0a6']!(this._ptr);

    return new TextPageNumberDescriptor(result);

  }


  
    black(): TextPageNumberDescriptor {


    const result = TextPageNumberDescriptorNativeFunctions['questpdf__text_page_number_descriptor__black__0cc8d698']!(this._ptr);

    return new TextPageNumberDescriptor(result);

  }


  
    extraBlack(): TextPageNumberDescriptor {


    const result = TextPageNumberDescriptorNativeFunctions['questpdf__text_page_number_descriptor__extra_black__c7698d85']!(this._ptr);

    return new TextPageNumberDescriptor(result);

  }


  
    normalPosition(): TextPageNumberDescriptor {


    const result = TextPageNumberDescriptorNativeFunctions['questpdf__text_page_number_descriptor__normal_position__5e5176cb']!(this._ptr);

    return new TextPageNumberDescriptor(result);

  }


  
    subscript(): TextPageNumberDescriptor {


    const result = TextPageNumberDescriptorNativeFunctions['questpdf__text_page_number_descriptor__subscript__db9bd4eb']!(this._ptr);

    return new TextPageNumberDescriptor(result);

  }


  
    superscript(): TextPageNumberDescriptor {


    const result = TextPageNumberDescriptorNativeFunctions['questpdf__text_page_number_descriptor__superscript__a9b46b1e']!(this._ptr);

    return new TextPageNumberDescriptor(result);

  }


  
    directionAuto(): TextPageNumberDescriptor {


    const result = TextPageNumberDescriptorNativeFunctions['questpdf__text_page_number_descriptor__direction_auto__fbed9e71']!(this._ptr);

    return new TextPageNumberDescriptor(result);

  }


  
    directionFromLeftToRight(): TextPageNumberDescriptor {


    const result = TextPageNumberDescriptorNativeFunctions['questpdf__text_page_number_descriptor__direction_from_left_to_right__09e2e3bc']!(this._ptr);

    return new TextPageNumberDescriptor(result);

  }


  
    directionFromRightToLeft(): TextPageNumberDescriptor {


    const result = TextPageNumberDescriptorNativeFunctions['questpdf__text_page_number_descriptor__direction_from_right_to_left__6cc5cb9e']!(this._ptr);

    return new TextPageNumberDescriptor(result);

  }


  
    enableFontFeature(featureName: string): TextPageNumberDescriptor {


    const result = TextPageNumberDescriptorNativeFunctions['questpdf__text_page_number_descriptor__enable_font_feature__136a164d']!(this._ptr, featureName);

    return new TextPageNumberDescriptor(result);

  }


  
    disableFontFeature(featureName: string): TextPageNumberDescriptor {


    const result = TextPageNumberDescriptorNativeFunctions['questpdf__text_page_number_descriptor__disable_font_feature__5bd81de9']!(this._ptr, featureName);

    return new TextPageNumberDescriptor(result);

  }




}


// ============================================================================
// TextBlockDescriptor - Native Function Declarations
// ============================================================================





const TextBlockDescriptorNativeFunctions: Record<string, ((...args: any[]) => any) | null> = {};

function initTextBlockDescriptorFunctions(lib: koffi.IKoffiLib): void {

  TextBlockDescriptorNativeFunctions['questpdf__text_block_descriptor__align_left__da139fee'] = lib.func('void* questpdf__text_block_descriptor__align_left__da139fee(void* target)');

  TextBlockDescriptorNativeFunctions['questpdf__text_block_descriptor__align_center__3661d942'] = lib.func('void* questpdf__text_block_descriptor__align_center__3661d942(void* target)');

  TextBlockDescriptorNativeFunctions['questpdf__text_block_descriptor__align_right__28e79232'] = lib.func('void* questpdf__text_block_descriptor__align_right__28e79232(void* target)');

  TextBlockDescriptorNativeFunctions['questpdf__text_block_descriptor__justify__f4a5d951'] = lib.func('void* questpdf__text_block_descriptor__justify__f4a5d951(void* target)');

  TextBlockDescriptorNativeFunctions['questpdf__text_block_descriptor__align_start__c97cfc2b'] = lib.func('void* questpdf__text_block_descriptor__align_start__c97cfc2b(void* target)');

  TextBlockDescriptorNativeFunctions['questpdf__text_block_descriptor__align_end__e0ace0c1'] = lib.func('void* questpdf__text_block_descriptor__align_end__e0ace0c1(void* target)');

  TextBlockDescriptorNativeFunctions['questpdf__text_block_descriptor__clamp_lines__2a5db05c'] = lib.func('void* questpdf__text_block_descriptor__clamp_lines__2a5db05c(void* target, int32_t maxLines, str16 ellipsis)');

  TextBlockDescriptorNativeFunctions['questpdf__text_block_descriptor__paragraph_spacing__6dcddcea'] = lib.func('void* questpdf__text_block_descriptor__paragraph_spacing__6dcddcea(void* target, float value, int32_t unit)');

  TextBlockDescriptorNativeFunctions['questpdf__text_block_descriptor__paragraph_first_line_indentation__5d11cadd'] = lib.func('void* questpdf__text_block_descriptor__paragraph_first_line_indentation__5d11cadd(void* target, float value, int32_t unit)');

  TextBlockDescriptorNativeFunctions['questpdf__text_block_descriptor__font_color__a0d06e42'] = lib.func('void* questpdf__text_block_descriptor__font_color__a0d06e42(void* target, uint32_t color)');

  TextBlockDescriptorNativeFunctions['questpdf__text_block_descriptor__background_color__5461b453'] = lib.func('void* questpdf__text_block_descriptor__background_color__5461b453(void* target, uint32_t color)');

  TextBlockDescriptorNativeFunctions['questpdf__text_block_descriptor__font_size__c989487d'] = lib.func('void* questpdf__text_block_descriptor__font_size__c989487d(void* target, float value)');

  TextBlockDescriptorNativeFunctions['questpdf__text_block_descriptor__line_height__a1b4697a'] = lib.func('void* questpdf__text_block_descriptor__line_height__a1b4697a(void* target, void* factor)');

  TextBlockDescriptorNativeFunctions['questpdf__text_block_descriptor__letter_spacing__92f86a26'] = lib.func('void* questpdf__text_block_descriptor__letter_spacing__92f86a26(void* target, float factor)');

  TextBlockDescriptorNativeFunctions['questpdf__text_block_descriptor__word_spacing__1f794add'] = lib.func('void* questpdf__text_block_descriptor__word_spacing__1f794add(void* target, float factor)');

  TextBlockDescriptorNativeFunctions['questpdf__text_block_descriptor__italic__4f023aba'] = lib.func('void* questpdf__text_block_descriptor__italic__4f023aba(void* target, uint8_t value)');

  TextBlockDescriptorNativeFunctions['questpdf__text_block_descriptor__strikethrough__41841206'] = lib.func('void* questpdf__text_block_descriptor__strikethrough__41841206(void* target, uint8_t value)');

  TextBlockDescriptorNativeFunctions['questpdf__text_block_descriptor__underline__2e1ae473'] = lib.func('void* questpdf__text_block_descriptor__underline__2e1ae473(void* target, uint8_t value)');

  TextBlockDescriptorNativeFunctions['questpdf__text_block_descriptor__overline__add25860'] = lib.func('void* questpdf__text_block_descriptor__overline__add25860(void* target, uint8_t value)');

  TextBlockDescriptorNativeFunctions['questpdf__text_block_descriptor__decoration_color__5d18d151'] = lib.func('void* questpdf__text_block_descriptor__decoration_color__5d18d151(void* target, uint32_t color)');

  TextBlockDescriptorNativeFunctions['questpdf__text_block_descriptor__decoration_thickness__c7c23c84'] = lib.func('void* questpdf__text_block_descriptor__decoration_thickness__c7c23c84(void* target, float factor)');

  TextBlockDescriptorNativeFunctions['questpdf__text_block_descriptor__decoration_solid__f64746d1'] = lib.func('void* questpdf__text_block_descriptor__decoration_solid__f64746d1(void* target)');

  TextBlockDescriptorNativeFunctions['questpdf__text_block_descriptor__decoration_double__41cf8a18'] = lib.func('void* questpdf__text_block_descriptor__decoration_double__41cf8a18(void* target)');

  TextBlockDescriptorNativeFunctions['questpdf__text_block_descriptor__decoration_wavy__1761acf2'] = lib.func('void* questpdf__text_block_descriptor__decoration_wavy__1761acf2(void* target)');

  TextBlockDescriptorNativeFunctions['questpdf__text_block_descriptor__decoration_dotted__e940537a'] = lib.func('void* questpdf__text_block_descriptor__decoration_dotted__e940537a(void* target)');

  TextBlockDescriptorNativeFunctions['questpdf__text_block_descriptor__decoration_dashed__a85f7344'] = lib.func('void* questpdf__text_block_descriptor__decoration_dashed__a85f7344(void* target)');

  TextBlockDescriptorNativeFunctions['questpdf__text_block_descriptor__thin__e9036638'] = lib.func('void* questpdf__text_block_descriptor__thin__e9036638(void* target)');

  TextBlockDescriptorNativeFunctions['questpdf__text_block_descriptor__extra_light__33bbe020'] = lib.func('void* questpdf__text_block_descriptor__extra_light__33bbe020(void* target)');

  TextBlockDescriptorNativeFunctions['questpdf__text_block_descriptor__light__37ef1bc2'] = lib.func('void* questpdf__text_block_descriptor__light__37ef1bc2(void* target)');

  TextBlockDescriptorNativeFunctions['questpdf__text_block_descriptor__normal_weight__18d360b3'] = lib.func('void* questpdf__text_block_descriptor__normal_weight__18d360b3(void* target)');

  TextBlockDescriptorNativeFunctions['questpdf__text_block_descriptor__medium__5ef8b80e'] = lib.func('void* questpdf__text_block_descriptor__medium__5ef8b80e(void* target)');

  TextBlockDescriptorNativeFunctions['questpdf__text_block_descriptor__semi_bold__0b92f7b7'] = lib.func('void* questpdf__text_block_descriptor__semi_bold__0b92f7b7(void* target)');

  TextBlockDescriptorNativeFunctions['questpdf__text_block_descriptor__bold__0dfa9061'] = lib.func('void* questpdf__text_block_descriptor__bold__0dfa9061(void* target)');

  TextBlockDescriptorNativeFunctions['questpdf__text_block_descriptor__extra_bold__c4fbc0a6'] = lib.func('void* questpdf__text_block_descriptor__extra_bold__c4fbc0a6(void* target)');

  TextBlockDescriptorNativeFunctions['questpdf__text_block_descriptor__black__0cc8d698'] = lib.func('void* questpdf__text_block_descriptor__black__0cc8d698(void* target)');

  TextBlockDescriptorNativeFunctions['questpdf__text_block_descriptor__extra_black__c7698d85'] = lib.func('void* questpdf__text_block_descriptor__extra_black__c7698d85(void* target)');

  TextBlockDescriptorNativeFunctions['questpdf__text_block_descriptor__normal_position__5e5176cb'] = lib.func('void* questpdf__text_block_descriptor__normal_position__5e5176cb(void* target)');

  TextBlockDescriptorNativeFunctions['questpdf__text_block_descriptor__subscript__db9bd4eb'] = lib.func('void* questpdf__text_block_descriptor__subscript__db9bd4eb(void* target)');

  TextBlockDescriptorNativeFunctions['questpdf__text_block_descriptor__superscript__a9b46b1e'] = lib.func('void* questpdf__text_block_descriptor__superscript__a9b46b1e(void* target)');

  TextBlockDescriptorNativeFunctions['questpdf__text_block_descriptor__direction_auto__fbed9e71'] = lib.func('void* questpdf__text_block_descriptor__direction_auto__fbed9e71(void* target)');

  TextBlockDescriptorNativeFunctions['questpdf__text_block_descriptor__direction_from_left_to_right__09e2e3bc'] = lib.func('void* questpdf__text_block_descriptor__direction_from_left_to_right__09e2e3bc(void* target)');

  TextBlockDescriptorNativeFunctions['questpdf__text_block_descriptor__direction_from_right_to_left__6cc5cb9e'] = lib.func('void* questpdf__text_block_descriptor__direction_from_right_to_left__6cc5cb9e(void* target)');

  TextBlockDescriptorNativeFunctions['questpdf__text_block_descriptor__enable_font_feature__136a164d'] = lib.func('void* questpdf__text_block_descriptor__enable_font_feature__136a164d(void* target, str16 featureName)');

  TextBlockDescriptorNativeFunctions['questpdf__text_block_descriptor__disable_font_feature__5bd81de9'] = lib.func('void* questpdf__text_block_descriptor__disable_font_feature__5bd81de9(void* target, str16 featureName)');



}

// Register initialization with the global initializers
classInitializers.push((lib: koffi.IKoffiLib) => initTextBlockDescriptorFunctions(lib));

// ============================================================================
// TextBlockDescriptor Class
// ============================================================================

export class TextBlockDescriptor {
  private readonly _ptr: NativePointer;
  private readonly _callbacks: koffi.IKoffiRegisteredCallback[] = [];

  constructor(ptr: NativePointer) {
    this._ptr = ptr;
  }

  get pointer(): NativePointer {
    return this._ptr;
  }


  
    alignLeft(): TextBlockDescriptor {


    const result = TextBlockDescriptorNativeFunctions['questpdf__text_block_descriptor__align_left__da139fee']!(this._ptr);

    return new TextBlockDescriptor(result);

  }


  
    alignCenter(): TextBlockDescriptor {


    const result = TextBlockDescriptorNativeFunctions['questpdf__text_block_descriptor__align_center__3661d942']!(this._ptr);

    return new TextBlockDescriptor(result);

  }


  
    alignRight(): TextBlockDescriptor {


    const result = TextBlockDescriptorNativeFunctions['questpdf__text_block_descriptor__align_right__28e79232']!(this._ptr);

    return new TextBlockDescriptor(result);

  }


  
    justify(): TextBlockDescriptor {


    const result = TextBlockDescriptorNativeFunctions['questpdf__text_block_descriptor__justify__f4a5d951']!(this._ptr);

    return new TextBlockDescriptor(result);

  }


  
    alignStart(): TextBlockDescriptor {


    const result = TextBlockDescriptorNativeFunctions['questpdf__text_block_descriptor__align_start__c97cfc2b']!(this._ptr);

    return new TextBlockDescriptor(result);

  }


  
    alignEnd(): TextBlockDescriptor {


    const result = TextBlockDescriptorNativeFunctions['questpdf__text_block_descriptor__align_end__e0ace0c1']!(this._ptr);

    return new TextBlockDescriptor(result);

  }


  
    clampLines(maxLines: number, ellipsis: string = '…'): TextBlockDescriptor {


    const result = TextBlockDescriptorNativeFunctions['questpdf__text_block_descriptor__clamp_lines__2a5db05c']!(this._ptr, maxLines, ellipsis);

    return new TextBlockDescriptor(result);

  }


  
    paragraphSpacing(value: number, unit: Unit = Unit.Point): TextBlockDescriptor {


    const result = TextBlockDescriptorNativeFunctions['questpdf__text_block_descriptor__paragraph_spacing__6dcddcea']!(this._ptr, value, unit);

    return new TextBlockDescriptor(result);

  }


  
    paragraphFirstLineIndentation(value: number, unit: Unit = Unit.Point): TextBlockDescriptor {


    const result = TextBlockDescriptorNativeFunctions['questpdf__text_block_descriptor__paragraph_first_line_indentation__5d11cadd']!(this._ptr, value, unit);

    return new TextBlockDescriptor(result);

  }


  
    fontColor(color: Color): TextBlockDescriptor {


    const result = TextBlockDescriptorNativeFunctions['questpdf__text_block_descriptor__font_color__a0d06e42']!(this._ptr, color.hex);

    return new TextBlockDescriptor(result);

  }


  
    backgroundColor(color: Color): TextBlockDescriptor {


    const result = TextBlockDescriptorNativeFunctions['questpdf__text_block_descriptor__background_color__5461b453']!(this._ptr, color.hex);

    return new TextBlockDescriptor(result);

  }


  
    fontSize(value: number): TextBlockDescriptor {


    const result = TextBlockDescriptorNativeFunctions['questpdf__text_block_descriptor__font_size__c989487d']!(this._ptr, value);

    return new TextBlockDescriptor(result);

  }


  
    lineHeight(factor: unknown): TextBlockDescriptor {


    const result = TextBlockDescriptorNativeFunctions['questpdf__text_block_descriptor__line_height__a1b4697a']!(this._ptr, factor);

    return new TextBlockDescriptor(result);

  }


  
    letterSpacing(factor: number = 0): TextBlockDescriptor {


    const result = TextBlockDescriptorNativeFunctions['questpdf__text_block_descriptor__letter_spacing__92f86a26']!(this._ptr, factor);

    return new TextBlockDescriptor(result);

  }


  
    wordSpacing(factor: number = 0): TextBlockDescriptor {


    const result = TextBlockDescriptorNativeFunctions['questpdf__text_block_descriptor__word_spacing__1f794add']!(this._ptr, factor);

    return new TextBlockDescriptor(result);

  }


  
    italic(value: boolean = true): TextBlockDescriptor {


    const result = TextBlockDescriptorNativeFunctions['questpdf__text_block_descriptor__italic__4f023aba']!(this._ptr, value);

    return new TextBlockDescriptor(result);

  }


  
    strikethrough(value: boolean = true): TextBlockDescriptor {


    const result = TextBlockDescriptorNativeFunctions['questpdf__text_block_descriptor__strikethrough__41841206']!(this._ptr, value);

    return new TextBlockDescriptor(result);

  }


  
    underline(value: boolean = true): TextBlockDescriptor {


    const result = TextBlockDescriptorNativeFunctions['questpdf__text_block_descriptor__underline__2e1ae473']!(this._ptr, value);

    return new TextBlockDescriptor(result);

  }


  
    overline(value: boolean = true): TextBlockDescriptor {


    const result = TextBlockDescriptorNativeFunctions['questpdf__text_block_descriptor__overline__add25860']!(this._ptr, value);

    return new TextBlockDescriptor(result);

  }


  
    decorationColor(color: Color): TextBlockDescriptor {


    const result = TextBlockDescriptorNativeFunctions['questpdf__text_block_descriptor__decoration_color__5d18d151']!(this._ptr, color.hex);

    return new TextBlockDescriptor(result);

  }


  
    decorationThickness(factor: number): TextBlockDescriptor {


    const result = TextBlockDescriptorNativeFunctions['questpdf__text_block_descriptor__decoration_thickness__c7c23c84']!(this._ptr, factor);

    return new TextBlockDescriptor(result);

  }


  
    decorationSolid(): TextBlockDescriptor {


    const result = TextBlockDescriptorNativeFunctions['questpdf__text_block_descriptor__decoration_solid__f64746d1']!(this._ptr);

    return new TextBlockDescriptor(result);

  }


  
    decorationDouble(): TextBlockDescriptor {


    const result = TextBlockDescriptorNativeFunctions['questpdf__text_block_descriptor__decoration_double__41cf8a18']!(this._ptr);

    return new TextBlockDescriptor(result);

  }


  
    decorationWavy(): TextBlockDescriptor {


    const result = TextBlockDescriptorNativeFunctions['questpdf__text_block_descriptor__decoration_wavy__1761acf2']!(this._ptr);

    return new TextBlockDescriptor(result);

  }


  
    decorationDotted(): TextBlockDescriptor {


    const result = TextBlockDescriptorNativeFunctions['questpdf__text_block_descriptor__decoration_dotted__e940537a']!(this._ptr);

    return new TextBlockDescriptor(result);

  }


  
    decorationDashed(): TextBlockDescriptor {


    const result = TextBlockDescriptorNativeFunctions['questpdf__text_block_descriptor__decoration_dashed__a85f7344']!(this._ptr);

    return new TextBlockDescriptor(result);

  }


  
    thin(): TextBlockDescriptor {


    const result = TextBlockDescriptorNativeFunctions['questpdf__text_block_descriptor__thin__e9036638']!(this._ptr);

    return new TextBlockDescriptor(result);

  }


  
    extraLight(): TextBlockDescriptor {


    const result = TextBlockDescriptorNativeFunctions['questpdf__text_block_descriptor__extra_light__33bbe020']!(this._ptr);

    return new TextBlockDescriptor(result);

  }


  
    light(): TextBlockDescriptor {


    const result = TextBlockDescriptorNativeFunctions['questpdf__text_block_descriptor__light__37ef1bc2']!(this._ptr);

    return new TextBlockDescriptor(result);

  }


  
    normalWeight(): TextBlockDescriptor {


    const result = TextBlockDescriptorNativeFunctions['questpdf__text_block_descriptor__normal_weight__18d360b3']!(this._ptr);

    return new TextBlockDescriptor(result);

  }


  
    medium(): TextBlockDescriptor {


    const result = TextBlockDescriptorNativeFunctions['questpdf__text_block_descriptor__medium__5ef8b80e']!(this._ptr);

    return new TextBlockDescriptor(result);

  }


  
    semiBold(): TextBlockDescriptor {


    const result = TextBlockDescriptorNativeFunctions['questpdf__text_block_descriptor__semi_bold__0b92f7b7']!(this._ptr);

    return new TextBlockDescriptor(result);

  }


  
    bold(): TextBlockDescriptor {


    const result = TextBlockDescriptorNativeFunctions['questpdf__text_block_descriptor__bold__0dfa9061']!(this._ptr);

    return new TextBlockDescriptor(result);

  }


  
    extraBold(): TextBlockDescriptor {


    const result = TextBlockDescriptorNativeFunctions['questpdf__text_block_descriptor__extra_bold__c4fbc0a6']!(this._ptr);

    return new TextBlockDescriptor(result);

  }


  
    black(): TextBlockDescriptor {


    const result = TextBlockDescriptorNativeFunctions['questpdf__text_block_descriptor__black__0cc8d698']!(this._ptr);

    return new TextBlockDescriptor(result);

  }


  
    extraBlack(): TextBlockDescriptor {


    const result = TextBlockDescriptorNativeFunctions['questpdf__text_block_descriptor__extra_black__c7698d85']!(this._ptr);

    return new TextBlockDescriptor(result);

  }


  
    normalPosition(): TextBlockDescriptor {


    const result = TextBlockDescriptorNativeFunctions['questpdf__text_block_descriptor__normal_position__5e5176cb']!(this._ptr);

    return new TextBlockDescriptor(result);

  }


  
    subscript(): TextBlockDescriptor {


    const result = TextBlockDescriptorNativeFunctions['questpdf__text_block_descriptor__subscript__db9bd4eb']!(this._ptr);

    return new TextBlockDescriptor(result);

  }


  
    superscript(): TextBlockDescriptor {


    const result = TextBlockDescriptorNativeFunctions['questpdf__text_block_descriptor__superscript__a9b46b1e']!(this._ptr);

    return new TextBlockDescriptor(result);

  }


  
    directionAuto(): TextBlockDescriptor {


    const result = TextBlockDescriptorNativeFunctions['questpdf__text_block_descriptor__direction_auto__fbed9e71']!(this._ptr);

    return new TextBlockDescriptor(result);

  }


  
    directionFromLeftToRight(): TextBlockDescriptor {


    const result = TextBlockDescriptorNativeFunctions['questpdf__text_block_descriptor__direction_from_left_to_right__09e2e3bc']!(this._ptr);

    return new TextBlockDescriptor(result);

  }


  
    directionFromRightToLeft(): TextBlockDescriptor {


    const result = TextBlockDescriptorNativeFunctions['questpdf__text_block_descriptor__direction_from_right_to_left__6cc5cb9e']!(this._ptr);

    return new TextBlockDescriptor(result);

  }


  
    enableFontFeature(featureName: string): TextBlockDescriptor {


    const result = TextBlockDescriptorNativeFunctions['questpdf__text_block_descriptor__enable_font_feature__136a164d']!(this._ptr, featureName);

    return new TextBlockDescriptor(result);

  }


  
    disableFontFeature(featureName: string): TextBlockDescriptor {


    const result = TextBlockDescriptorNativeFunctions['questpdf__text_block_descriptor__disable_font_feature__5bd81de9']!(this._ptr, featureName);

    return new TextBlockDescriptor(result);

  }




}


// ============================================================================
// ImageDescriptor - Native Function Declarations
// ============================================================================





const ImageDescriptorNativeFunctions: Record<string, ((...args: any[]) => any) | null> = {};

function initImageDescriptorFunctions(lib: koffi.IKoffiLib): void {

  ImageDescriptorNativeFunctions['questpdf__image_descriptor__use_original_image__d3be84c2'] = lib.func('void* questpdf__image_descriptor__use_original_image__d3be84c2(void* target, uint8_t value)');

  ImageDescriptorNativeFunctions['questpdf__image_descriptor__with_raster_dpi__78f617ee'] = lib.func('void* questpdf__image_descriptor__with_raster_dpi__78f617ee(void* target, int32_t dpi)');

  ImageDescriptorNativeFunctions['questpdf__image_descriptor__with_compression_quality__1665a445'] = lib.func('void* questpdf__image_descriptor__with_compression_quality__1665a445(void* target, int32_t quality)');

  ImageDescriptorNativeFunctions['questpdf__image_descriptor__fit_width__7b9aa4d6'] = lib.func('void* questpdf__image_descriptor__fit_width__7b9aa4d6(void* target)');

  ImageDescriptorNativeFunctions['questpdf__image_descriptor__fit_height__c888daad'] = lib.func('void* questpdf__image_descriptor__fit_height__c888daad(void* target)');

  ImageDescriptorNativeFunctions['questpdf__image_descriptor__fit_area__4dbf28f5'] = lib.func('void* questpdf__image_descriptor__fit_area__4dbf28f5(void* target)');

  ImageDescriptorNativeFunctions['questpdf__image_descriptor__fit_unproportionally__3d7bad76'] = lib.func('void* questpdf__image_descriptor__fit_unproportionally__3d7bad76(void* target)');



}

// Register initialization with the global initializers
classInitializers.push((lib: koffi.IKoffiLib) => initImageDescriptorFunctions(lib));

// ============================================================================
// ImageDescriptor Class
// ============================================================================

export class ImageDescriptor {
  private readonly _ptr: NativePointer;
  private readonly _callbacks: koffi.IKoffiRegisteredCallback[] = [];

  constructor(ptr: NativePointer) {
    this._ptr = ptr;
  }

  get pointer(): NativePointer {
    return this._ptr;
  }


  
    useOriginalImage(value: boolean = true): ImageDescriptor {


    const result = ImageDescriptorNativeFunctions['questpdf__image_descriptor__use_original_image__d3be84c2']!(this._ptr, value);

    return new ImageDescriptor(result);

  }


  
    withRasterDpi(dpi: number): ImageDescriptor {


    const result = ImageDescriptorNativeFunctions['questpdf__image_descriptor__with_raster_dpi__78f617ee']!(this._ptr, dpi);

    return new ImageDescriptor(result);

  }


  
    withCompressionQuality(quality: ImageCompressionQuality): ImageDescriptor {


    const result = ImageDescriptorNativeFunctions['questpdf__image_descriptor__with_compression_quality__1665a445']!(this._ptr, quality);

    return new ImageDescriptor(result);

  }


  
    fitWidth(): ImageDescriptor {


    const result = ImageDescriptorNativeFunctions['questpdf__image_descriptor__fit_width__7b9aa4d6']!(this._ptr);

    return new ImageDescriptor(result);

  }


  
    fitHeight(): ImageDescriptor {


    const result = ImageDescriptorNativeFunctions['questpdf__image_descriptor__fit_height__c888daad']!(this._ptr);

    return new ImageDescriptor(result);

  }


  
    fitArea(): ImageDescriptor {


    const result = ImageDescriptorNativeFunctions['questpdf__image_descriptor__fit_area__4dbf28f5']!(this._ptr);

    return new ImageDescriptor(result);

  }


  
    fitUnproportionally(): ImageDescriptor {


    const result = ImageDescriptorNativeFunctions['questpdf__image_descriptor__fit_unproportionally__3d7bad76']!(this._ptr);

    return new ImageDescriptor(result);

  }




}


// ============================================================================
// DynamicImageDescriptor - Native Function Declarations
// ============================================================================





const DynamicImageDescriptorNativeFunctions: Record<string, ((...args: any[]) => any) | null> = {};

function initDynamicImageDescriptorFunctions(lib: koffi.IKoffiLib): void {

  DynamicImageDescriptorNativeFunctions['questpdf__dynamic_image_descriptor__use_original_image__1dabc6b8'] = lib.func('void* questpdf__dynamic_image_descriptor__use_original_image__1dabc6b8(void* target, uint8_t value)');

  DynamicImageDescriptorNativeFunctions['questpdf__dynamic_image_descriptor__with_raster_dpi__a72018d5'] = lib.func('void* questpdf__dynamic_image_descriptor__with_raster_dpi__a72018d5(void* target, int32_t dpi)');

  DynamicImageDescriptorNativeFunctions['questpdf__dynamic_image_descriptor__with_compression_quality__94465629'] = lib.func('void* questpdf__dynamic_image_descriptor__with_compression_quality__94465629(void* target, int32_t quality)');



}

// Register initialization with the global initializers
classInitializers.push((lib: koffi.IKoffiLib) => initDynamicImageDescriptorFunctions(lib));

// ============================================================================
// DynamicImageDescriptor Class
// ============================================================================

export class DynamicImageDescriptor {
  private readonly _ptr: NativePointer;
  private readonly _callbacks: koffi.IKoffiRegisteredCallback[] = [];

  constructor(ptr: NativePointer) {
    this._ptr = ptr;
  }

  get pointer(): NativePointer {
    return this._ptr;
  }


  
    useOriginalImage(value: boolean = true): DynamicImageDescriptor {


    const result = DynamicImageDescriptorNativeFunctions['questpdf__dynamic_image_descriptor__use_original_image__1dabc6b8']!(this._ptr, value);

    return new DynamicImageDescriptor(result);

  }


  
    withRasterDpi(dpi: number): DynamicImageDescriptor {


    const result = DynamicImageDescriptorNativeFunctions['questpdf__dynamic_image_descriptor__with_raster_dpi__a72018d5']!(this._ptr, dpi);

    return new DynamicImageDescriptor(result);

  }


  
    withCompressionQuality(quality: ImageCompressionQuality): DynamicImageDescriptor {


    const result = DynamicImageDescriptorNativeFunctions['questpdf__dynamic_image_descriptor__with_compression_quality__94465629']!(this._ptr, quality);

    return new DynamicImageDescriptor(result);

  }




}


// ============================================================================
// SvgImageDescriptor - Native Function Declarations
// ============================================================================





const SvgImageDescriptorNativeFunctions: Record<string, ((...args: any[]) => any) | null> = {};

function initSvgImageDescriptorFunctions(lib: koffi.IKoffiLib): void {

  SvgImageDescriptorNativeFunctions['questpdf__svg_image_descriptor__fit_width__ae37e277'] = lib.func('void* questpdf__svg_image_descriptor__fit_width__ae37e277(void* target)');

  SvgImageDescriptorNativeFunctions['questpdf__svg_image_descriptor__fit_height__7e11f169'] = lib.func('void* questpdf__svg_image_descriptor__fit_height__7e11f169(void* target)');

  SvgImageDescriptorNativeFunctions['questpdf__svg_image_descriptor__fit_area__6abce393'] = lib.func('void* questpdf__svg_image_descriptor__fit_area__6abce393(void* target)');



}

// Register initialization with the global initializers
classInitializers.push((lib: koffi.IKoffiLib) => initSvgImageDescriptorFunctions(lib));

// ============================================================================
// SvgImageDescriptor Class
// ============================================================================

export class SvgImageDescriptor {
  private readonly _ptr: NativePointer;
  private readonly _callbacks: koffi.IKoffiRegisteredCallback[] = [];

  constructor(ptr: NativePointer) {
    this._ptr = ptr;
  }

  get pointer(): NativePointer {
    return this._ptr;
  }


  
    fitWidth(): SvgImageDescriptor {


    const result = SvgImageDescriptorNativeFunctions['questpdf__svg_image_descriptor__fit_width__ae37e277']!(this._ptr);

    return new SvgImageDescriptor(result);

  }


  
    fitHeight(): SvgImageDescriptor {


    const result = SvgImageDescriptorNativeFunctions['questpdf__svg_image_descriptor__fit_height__7e11f169']!(this._ptr);

    return new SvgImageDescriptor(result);

  }


  
    fitArea(): SvgImageDescriptor {


    const result = SvgImageDescriptorNativeFunctions['questpdf__svg_image_descriptor__fit_area__6abce393']!(this._ptr);

    return new SvgImageDescriptor(result);

  }




}


// ============================================================================
// PageDescriptor - Native Function Declarations
// ============================================================================





const PageDescriptorNativeFunctions: Record<string, ((...args: any[]) => any) | null> = {};

function initPageDescriptorFunctions(lib: koffi.IKoffiLib): void {

  PageDescriptorNativeFunctions['questpdf__page_descriptor__size__cd4bd97a'] = lib.func('void questpdf__page_descriptor__size__cd4bd97a(void* target, float width, float height, int32_t unit)');

  PageDescriptorNativeFunctions['questpdf__page_descriptor__size__593af4d7'] = lib.func('void questpdf__page_descriptor__size__593af4d7(void* target, void* pageSize)');

  PageDescriptorNativeFunctions['questpdf__page_descriptor__continuous_size__ae1c9536'] = lib.func('void questpdf__page_descriptor__continuous_size__ae1c9536(void* target, float width, int32_t unit)');

  PageDescriptorNativeFunctions['questpdf__page_descriptor__min_size__8216ba5f'] = lib.func('void questpdf__page_descriptor__min_size__8216ba5f(void* target, void* pageSize)');

  PageDescriptorNativeFunctions['questpdf__page_descriptor__max_size__02a75d07'] = lib.func('void questpdf__page_descriptor__max_size__02a75d07(void* target, void* pageSize)');

  PageDescriptorNativeFunctions['questpdf__page_descriptor__margin_left__4c6b8a4c'] = lib.func('void questpdf__page_descriptor__margin_left__4c6b8a4c(void* target, float value, int32_t unit)');

  PageDescriptorNativeFunctions['questpdf__page_descriptor__margin_right__de714424'] = lib.func('void questpdf__page_descriptor__margin_right__de714424(void* target, float value, int32_t unit)');

  PageDescriptorNativeFunctions['questpdf__page_descriptor__margin_top__51f73aad'] = lib.func('void questpdf__page_descriptor__margin_top__51f73aad(void* target, float value, int32_t unit)');

  PageDescriptorNativeFunctions['questpdf__page_descriptor__margin_bottom__d37c07b0'] = lib.func('void questpdf__page_descriptor__margin_bottom__d37c07b0(void* target, float value, int32_t unit)');

  PageDescriptorNativeFunctions['questpdf__page_descriptor__margin_vertical__97f4d868'] = lib.func('void questpdf__page_descriptor__margin_vertical__97f4d868(void* target, float value, int32_t unit)');

  PageDescriptorNativeFunctions['questpdf__page_descriptor__margin_horizontal__bafb50fd'] = lib.func('void questpdf__page_descriptor__margin_horizontal__bafb50fd(void* target, float value, int32_t unit)');

  PageDescriptorNativeFunctions['questpdf__page_descriptor__margin__35d8c8b5'] = lib.func('void questpdf__page_descriptor__margin__35d8c8b5(void* target, float value, int32_t unit)');

  PageDescriptorNativeFunctions['questpdf__page_descriptor__content_from_left_to_right__6bcc64b0'] = lib.func('void questpdf__page_descriptor__content_from_left_to_right__6bcc64b0(void* target)');

  PageDescriptorNativeFunctions['questpdf__page_descriptor__content_from_right_to_left__dbce6933'] = lib.func('void questpdf__page_descriptor__content_from_right_to_left__dbce6933(void* target)');

  PageDescriptorNativeFunctions['questpdf__page_descriptor__page_color__7ab89cfe'] = lib.func('void questpdf__page_descriptor__page_color__7ab89cfe(void* target, uint32_t color)');

  PageDescriptorNativeFunctions['questpdf__page_descriptor__background__8048fed3'] = lib.func('void* questpdf__page_descriptor__background__8048fed3(void* target)');

  PageDescriptorNativeFunctions['questpdf__page_descriptor__foreground__b6d36c4d'] = lib.func('void* questpdf__page_descriptor__foreground__b6d36c4d(void* target)');

  PageDescriptorNativeFunctions['questpdf__page_descriptor__header__392dd2be'] = lib.func('void* questpdf__page_descriptor__header__392dd2be(void* target)');

  PageDescriptorNativeFunctions['questpdf__page_descriptor__content__af503598'] = lib.func('void* questpdf__page_descriptor__content__af503598(void* target)');

  PageDescriptorNativeFunctions['questpdf__page_descriptor__footer__eb07832c'] = lib.func('void* questpdf__page_descriptor__footer__eb07832c(void* target)');



}

// Register initialization with the global initializers
classInitializers.push((lib: koffi.IKoffiLib) => initPageDescriptorFunctions(lib));

// ============================================================================
// PageDescriptor Class
// ============================================================================

export class PageDescriptor {
  private readonly _ptr: NativePointer;
  private readonly _callbacks: koffi.IKoffiRegisteredCallback[] = [];

  constructor(ptr: NativePointer) {
    this._ptr = ptr;
  }

  get pointer(): NativePointer {
    return this._ptr;
  }


  
    private size_Float_Float_Unit(width: number, height: number, unit: Unit = Unit.Point): void {


    const result = PageDescriptorNativeFunctions['questpdf__page_descriptor__size__cd4bd97a']!(this._ptr, width, height, unit);

  }


  
    private size_PageSize(pageSize: PageSize): void {


    const result = PageDescriptorNativeFunctions['questpdf__page_descriptor__size__593af4d7']!(this._ptr, pageSize.pointer);

  }


  
    continuousSize(width: number, unit: Unit = Unit.Point): void {


    const result = PageDescriptorNativeFunctions['questpdf__page_descriptor__continuous_size__ae1c9536']!(this._ptr, width, unit);

  }


  
    minSize(pageSize: PageSize): void {


    const result = PageDescriptorNativeFunctions['questpdf__page_descriptor__min_size__8216ba5f']!(this._ptr, pageSize.pointer);

  }


  
    maxSize(pageSize: PageSize): void {


    const result = PageDescriptorNativeFunctions['questpdf__page_descriptor__max_size__02a75d07']!(this._ptr, pageSize.pointer);

  }


  
    marginLeft(value: number, unit: Unit = Unit.Point): void {


    const result = PageDescriptorNativeFunctions['questpdf__page_descriptor__margin_left__4c6b8a4c']!(this._ptr, value, unit);

  }


  
    marginRight(value: number, unit: Unit = Unit.Point): void {


    const result = PageDescriptorNativeFunctions['questpdf__page_descriptor__margin_right__de714424']!(this._ptr, value, unit);

  }


  
    marginTop(value: number, unit: Unit = Unit.Point): void {


    const result = PageDescriptorNativeFunctions['questpdf__page_descriptor__margin_top__51f73aad']!(this._ptr, value, unit);

  }


  
    marginBottom(value: number, unit: Unit = Unit.Point): void {


    const result = PageDescriptorNativeFunctions['questpdf__page_descriptor__margin_bottom__d37c07b0']!(this._ptr, value, unit);

  }


  
    marginVertical(value: number, unit: Unit = Unit.Point): void {


    const result = PageDescriptorNativeFunctions['questpdf__page_descriptor__margin_vertical__97f4d868']!(this._ptr, value, unit);

  }


  
    marginHorizontal(value: number, unit: Unit = Unit.Point): void {


    const result = PageDescriptorNativeFunctions['questpdf__page_descriptor__margin_horizontal__bafb50fd']!(this._ptr, value, unit);

  }


  
    margin(value: number, unit: Unit = Unit.Point): void {


    const result = PageDescriptorNativeFunctions['questpdf__page_descriptor__margin__35d8c8b5']!(this._ptr, value, unit);

  }


  
    contentFromLeftToRight(): void {


    const result = PageDescriptorNativeFunctions['questpdf__page_descriptor__content_from_left_to_right__6bcc64b0']!(this._ptr);

  }


  
    contentFromRightToLeft(): void {


    const result = PageDescriptorNativeFunctions['questpdf__page_descriptor__content_from_right_to_left__dbce6933']!(this._ptr);

  }


  
    pageColor(color: Color): void {


    const result = PageDescriptorNativeFunctions['questpdf__page_descriptor__page_color__7ab89cfe']!(this._ptr, color.hex);

  }


  
    background(): Container {


    const result = PageDescriptorNativeFunctions['questpdf__page_descriptor__background__8048fed3']!(this._ptr);

    return new Container(result);

  }


  
    foreground(): Container {


    const result = PageDescriptorNativeFunctions['questpdf__page_descriptor__foreground__b6d36c4d']!(this._ptr);

    return new Container(result);

  }


  
    header(): Container {


    const result = PageDescriptorNativeFunctions['questpdf__page_descriptor__header__392dd2be']!(this._ptr);

    return new Container(result);

  }


  
    content(): Container {


    const result = PageDescriptorNativeFunctions['questpdf__page_descriptor__content__af503598']!(this._ptr);

    return new Container(result);

  }


  
    footer(): Container {


    const result = PageDescriptorNativeFunctions['questpdf__page_descriptor__footer__eb07832c']!(this._ptr);

    return new Container(result);

  }




}


// ============================================================================
// Container - Native Function Declarations
// ============================================================================




// typedef void (*column_descriptor_callback_24d6ceed)(void*);

// typedef void (*decoration_descriptor_callback_0b39c58e)(void*);

// typedef void (*container_callback_971e7b54)(void*);

// typedef void (*container_callback_a33b5f9b)(void*);

// typedef void (*inlined_descriptor_callback_33b27c8d)(void*);

// typedef void (*layers_descriptor_callback_03ce5bdd)(void*);

// typedef void (*multi_column_descriptor_callback_193479d6)(void*);

// typedef void (*row_descriptor_callback_39fce557)(void*);

// typedef void (*table_descriptor_callback_d49da987)(void*);

// typedef void (*text_descriptor_callback_357e362f)(void*);


const ContainerNativeFunctions: Record<string, ((...args: any[]) => any) | null> = {};

function initContainerFunctions(lib: koffi.IKoffiLib): void {

  ContainerNativeFunctions['questpdf__container__align_left__68bfdc67'] = lib.func('void* questpdf__container__align_left__68bfdc67(void* target)');

  ContainerNativeFunctions['questpdf__container__align_center__4fb1e0d1'] = lib.func('void* questpdf__container__align_center__4fb1e0d1(void* target)');

  ContainerNativeFunctions['questpdf__container__align_right__a1c1a1bf'] = lib.func('void* questpdf__container__align_right__a1c1a1bf(void* target)');

  ContainerNativeFunctions['questpdf__container__align_top__f275ca95'] = lib.func('void* questpdf__container__align_top__f275ca95(void* target)');

  ContainerNativeFunctions['questpdf__container__align_middle__95fef9e8'] = lib.func('void* questpdf__container__align_middle__95fef9e8(void* target)');

  ContainerNativeFunctions['questpdf__container__align_bottom__d33d0520'] = lib.func('void* questpdf__container__align_bottom__d33d0520(void* target)');

  ContainerNativeFunctions['questpdf__container__column__24d6ceed'] = lib.func('void questpdf__container__column__24d6ceed(void* target, void* handler)');

  ContainerNativeFunctions['questpdf__container__width__a346e20f'] = lib.func('void* questpdf__container__width__a346e20f(void* target, float value, int32_t unit)');

  ContainerNativeFunctions['questpdf__container__min_width__c00f1915'] = lib.func('void* questpdf__container__min_width__c00f1915(void* target, float value, int32_t unit)');

  ContainerNativeFunctions['questpdf__container__max_width__7e85a057'] = lib.func('void* questpdf__container__max_width__7e85a057(void* target, float value, int32_t unit)');

  ContainerNativeFunctions['questpdf__container__height__36ac3a02'] = lib.func('void* questpdf__container__height__36ac3a02(void* target, float value, int32_t unit)');

  ContainerNativeFunctions['questpdf__container__min_height__58cc06b0'] = lib.func('void* questpdf__container__min_height__58cc06b0(void* target, float value, int32_t unit)');

  ContainerNativeFunctions['questpdf__container__max_height__0b76e199'] = lib.func('void* questpdf__container__max_height__0b76e199(void* target, float value, int32_t unit)');

  ContainerNativeFunctions['questpdf__container__content_from_left_to_right__191523c1'] = lib.func('void* questpdf__container__content_from_left_to_right__191523c1(void* target)');

  ContainerNativeFunctions['questpdf__container__content_from_right_to_left__a31dbd9d'] = lib.func('void* questpdf__container__content_from_right_to_left__a31dbd9d(void* target)');

  ContainerNativeFunctions['questpdf__container__debug_area__a69b9c65'] = lib.func('void* questpdf__container__debug_area__a69b9c65(void* target, str16 text, uint32_t color)');

  ContainerNativeFunctions['questpdf__container__debug_pointer__9d669879'] = lib.func('void* questpdf__container__debug_pointer__9d669879(void* target, str16 label)');

  ContainerNativeFunctions['questpdf__container__decoration__0b39c58e'] = lib.func('void questpdf__container__decoration__0b39c58e(void* target, void* handler)');

  ContainerNativeFunctions['questpdf__container__aspect_ratio__fd5bc0dc'] = lib.func('void* questpdf__container__aspect_ratio__fd5bc0dc(void* target, float ratio, int32_t option)');

  ContainerNativeFunctions['questpdf__container__placeholder__a560192f'] = lib.func('void questpdf__container__placeholder__a560192f(void* target, str16 text)');

  ContainerNativeFunctions['questpdf__container__show_once__c6224013'] = lib.func('void* questpdf__container__show_once__c6224013(void* target)');

  ContainerNativeFunctions['questpdf__container__skip_once__b3d4c7bf'] = lib.func('void* questpdf__container__skip_once__b3d4c7bf(void* target)');

  ContainerNativeFunctions['questpdf__container__show_entire__16629c88'] = lib.func('void* questpdf__container__show_entire__16629c88(void* target)');

  ContainerNativeFunctions['questpdf__container__ensure_space__0cbedd6a'] = lib.func('void* questpdf__container__ensure_space__0cbedd6a(void* target, float minHeight)');

  ContainerNativeFunctions['questpdf__container__prevent_page_break__2e3cab6a'] = lib.func('void* questpdf__container__prevent_page_break__2e3cab6a(void* target)');

  ContainerNativeFunctions['questpdf__container__page_break__4287fb55'] = lib.func('void questpdf__container__page_break__4287fb55(void* target)');

  ContainerNativeFunctions['questpdf__container__container__be126adc'] = lib.func('void* questpdf__container__container__be126adc(void* target)');

  ContainerNativeFunctions['questpdf__container__hyperlink__40aee55c'] = lib.func('void* questpdf__container__hyperlink__40aee55c(void* target, str16 url)');

  ContainerNativeFunctions['questpdf__container__section__b2687119'] = lib.func('void* questpdf__container__section__b2687119(void* target, str16 sectionName)');

  ContainerNativeFunctions['questpdf__container__section_link__d27b4828'] = lib.func('void* questpdf__container__section_link__d27b4828(void* target, str16 sectionName)');

  ContainerNativeFunctions['questpdf__container__show_if__da52e306'] = lib.func('void* questpdf__container__show_if__da52e306(void* target, uint8_t condition)');

  ContainerNativeFunctions['questpdf__container__unconstrained__a43107f6'] = lib.func('void* questpdf__container__unconstrained__a43107f6(void* target)');

  ContainerNativeFunctions['questpdf__container__stop_paging__81b05f34'] = lib.func('void* questpdf__container__stop_paging__81b05f34(void* target)');

  ContainerNativeFunctions['questpdf__container__scale_to_fit__bb0b4e57'] = lib.func('void* questpdf__container__scale_to_fit__bb0b4e57(void* target)');

  ContainerNativeFunctions['questpdf__container__repeat__e198bc84'] = lib.func('void* questpdf__container__repeat__e198bc84(void* target)');

  ContainerNativeFunctions['questpdf__container__lazy__971e7b54'] = lib.func('void questpdf__container__lazy__971e7b54(void* target, void* contentBuilder)');

  ContainerNativeFunctions['questpdf__container__lazy_with_cache__a33b5f9b'] = lib.func('void questpdf__container__lazy_with_cache__a33b5f9b(void* target, void* contentBuilder)');

  ContainerNativeFunctions['questpdf__container__z_index__9cd9a32e'] = lib.func('void* questpdf__container__z_index__9cd9a32e(void* target, int32_t indexValue)');

  ContainerNativeFunctions['questpdf__container__capture_content_position__845fb313'] = lib.func('void* questpdf__container__capture_content_position__845fb313(void* target, str16 id)');

  ContainerNativeFunctions['questpdf__container__extend__291e835a'] = lib.func('void* questpdf__container__extend__291e835a(void* target)');

  ContainerNativeFunctions['questpdf__container__extend_vertical__e63e1d72'] = lib.func('void* questpdf__container__extend_vertical__e63e1d72(void* target)');

  ContainerNativeFunctions['questpdf__container__extend_horizontal__c6d6d128'] = lib.func('void* questpdf__container__extend_horizontal__c6d6d128(void* target)');

  ContainerNativeFunctions['questpdf__container__image__9155d14a'] = lib.func('void* questpdf__container__image__9155d14a(void* target, str16 filePath)');

  ContainerNativeFunctions['questpdf__container__image__ccf976d1'] = lib.func('void* questpdf__container__image__ccf976d1(void* target, void* image)');

  ContainerNativeFunctions['questpdf__container__inlined__33b27c8d'] = lib.func('void questpdf__container__inlined__33b27c8d(void* target, void* handler)');

  ContainerNativeFunctions['questpdf__container__layers__03ce5bdd'] = lib.func('void questpdf__container__layers__03ce5bdd(void* target, void* handler)');

  ContainerNativeFunctions['questpdf__container__line_vertical__ab97b857'] = lib.func('void* questpdf__container__line_vertical__ab97b857(void* target, float thickness, int32_t unit)');

  ContainerNativeFunctions['questpdf__container__line_horizontal__a6f7f11f'] = lib.func('void* questpdf__container__line_horizontal__a6f7f11f(void* target, float thickness, int32_t unit)');

  ContainerNativeFunctions['questpdf__container__multi_column__193479d6'] = lib.func('void questpdf__container__multi_column__193479d6(void* target, void* handler)');

  ContainerNativeFunctions['questpdf__container__padding__5daecb7e'] = lib.func('void* questpdf__container__padding__5daecb7e(void* target, float value, int32_t unit)');

  ContainerNativeFunctions['questpdf__container__padding_horizontal__7a6b255d'] = lib.func('void* questpdf__container__padding_horizontal__7a6b255d(void* target, float value, int32_t unit)');

  ContainerNativeFunctions['questpdf__container__padding_vertical__91122aaa'] = lib.func('void* questpdf__container__padding_vertical__91122aaa(void* target, float value, int32_t unit)');

  ContainerNativeFunctions['questpdf__container__padding_top__de3b7b3b'] = lib.func('void* questpdf__container__padding_top__de3b7b3b(void* target, float value, int32_t unit)');

  ContainerNativeFunctions['questpdf__container__padding_bottom__74ad0a7b'] = lib.func('void* questpdf__container__padding_bottom__74ad0a7b(void* target, float value, int32_t unit)');

  ContainerNativeFunctions['questpdf__container__padding_left__103ee738'] = lib.func('void* questpdf__container__padding_left__103ee738(void* target, float value, int32_t unit)');

  ContainerNativeFunctions['questpdf__container__padding_right__89d1cf61'] = lib.func('void* questpdf__container__padding_right__89d1cf61(void* target, float value, int32_t unit)');

  ContainerNativeFunctions['questpdf__container__rotate_left__c5193e66'] = lib.func('void* questpdf__container__rotate_left__c5193e66(void* target)');

  ContainerNativeFunctions['questpdf__container__rotate_right__004c9c52'] = lib.func('void* questpdf__container__rotate_right__004c9c52(void* target)');

  ContainerNativeFunctions['questpdf__container__rotate__c33f62ac'] = lib.func('void* questpdf__container__rotate__c33f62ac(void* target, float angle)');

  ContainerNativeFunctions['questpdf__container__row__39fce557'] = lib.func('void questpdf__container__row__39fce557(void* target, void* handler)');

  ContainerNativeFunctions['questpdf__container__scale__05521931'] = lib.func('void* questpdf__container__scale__05521931(void* target, float factor)');

  ContainerNativeFunctions['questpdf__container__scale_horizontal__14d1a9be'] = lib.func('void* questpdf__container__scale_horizontal__14d1a9be(void* target, float factor)');

  ContainerNativeFunctions['questpdf__container__scale_vertical__5bc8a8a5'] = lib.func('void* questpdf__container__scale_vertical__5bc8a8a5(void* target, float factor)');

  ContainerNativeFunctions['questpdf__container__flip_horizontal__744e4fe9'] = lib.func('void* questpdf__container__flip_horizontal__744e4fe9(void* target)');

  ContainerNativeFunctions['questpdf__container__flip_vertical__a91487f3'] = lib.func('void* questpdf__container__flip_vertical__a91487f3(void* target)');

  ContainerNativeFunctions['questpdf__container__flip_over__ce1ff345'] = lib.func('void* questpdf__container__flip_over__ce1ff345(void* target)');

  ContainerNativeFunctions['questpdf__container__shrink__4221b85b'] = lib.func('void* questpdf__container__shrink__4221b85b(void* target)');

  ContainerNativeFunctions['questpdf__container__shrink_vertical__e5042c3c'] = lib.func('void* questpdf__container__shrink_vertical__e5042c3c(void* target)');

  ContainerNativeFunctions['questpdf__container__shrink_horizontal__588cfd0f'] = lib.func('void* questpdf__container__shrink_horizontal__588cfd0f(void* target)');

  ContainerNativeFunctions['questpdf__container__border__a6712928'] = lib.func('void* questpdf__container__border__a6712928(void* target, float all, uint32_t color)');

  ContainerNativeFunctions['questpdf__container__background__68f98b81'] = lib.func('void* questpdf__container__background__68f98b81(void* target, uint32_t color)');

  ContainerNativeFunctions['questpdf__container__border__17f3b5e4'] = lib.func('void* questpdf__container__border__17f3b5e4(void* target, float value, int32_t unit)');

  ContainerNativeFunctions['questpdf__container__border_vertical__7922384b'] = lib.func('void* questpdf__container__border_vertical__7922384b(void* target, float value, int32_t unit)');

  ContainerNativeFunctions['questpdf__container__border_horizontal__34913f34'] = lib.func('void* questpdf__container__border_horizontal__34913f34(void* target, float value, int32_t unit)');

  ContainerNativeFunctions['questpdf__container__border_left__803ed1e6'] = lib.func('void* questpdf__container__border_left__803ed1e6(void* target, float value, int32_t unit)');

  ContainerNativeFunctions['questpdf__container__border_right__de8ca6bf'] = lib.func('void* questpdf__container__border_right__de8ca6bf(void* target, float value, int32_t unit)');

  ContainerNativeFunctions['questpdf__container__border_top__c469b91f'] = lib.func('void* questpdf__container__border_top__c469b91f(void* target, float value, int32_t unit)');

  ContainerNativeFunctions['questpdf__container__border_bottom__59b8a019'] = lib.func('void* questpdf__container__border_bottom__59b8a019(void* target, float value, int32_t unit)');

  ContainerNativeFunctions['questpdf__container__corner_radius__bf7cb39f'] = lib.func('void* questpdf__container__corner_radius__bf7cb39f(void* target, float value, int32_t unit)');

  ContainerNativeFunctions['questpdf__container__corner_radius_top_left__41d08c72'] = lib.func('void* questpdf__container__corner_radius_top_left__41d08c72(void* target, float value, int32_t unit)');

  ContainerNativeFunctions['questpdf__container__corner_radius_top_right__1497678a'] = lib.func('void* questpdf__container__corner_radius_top_right__1497678a(void* target, float value, int32_t unit)');

  ContainerNativeFunctions['questpdf__container__corner_radius_bottom_left__3a8d234a'] = lib.func('void* questpdf__container__corner_radius_bottom_left__3a8d234a(void* target, float value, int32_t unit)');

  ContainerNativeFunctions['questpdf__container__corner_radius_bottom_right__b07c1d8d'] = lib.func('void* questpdf__container__corner_radius_bottom_right__b07c1d8d(void* target, float value, int32_t unit)');

  ContainerNativeFunctions['questpdf__container__border_color__2a24bda0'] = lib.func('void* questpdf__container__border_color__2a24bda0(void* target, uint32_t color)');

  ContainerNativeFunctions['questpdf__container__border_alignment_outside__ce5e63fa'] = lib.func('void* questpdf__container__border_alignment_outside__ce5e63fa(void* target)');

  ContainerNativeFunctions['questpdf__container__border_alignment_middle__66a27445'] = lib.func('void* questpdf__container__border_alignment_middle__66a27445(void* target)');

  ContainerNativeFunctions['questpdf__container__border_alignment_inside__8cef56b1'] = lib.func('void* questpdf__container__border_alignment_inside__8cef56b1(void* target)');

  ContainerNativeFunctions['questpdf__container__svg__f547d46e'] = lib.func('void* questpdf__container__svg__f547d46e(void* target, str16 svg)');

  ContainerNativeFunctions['questpdf__container__svg__b1de06e3'] = lib.func('void* questpdf__container__svg__b1de06e3(void* target, void* image)');

  ContainerNativeFunctions['questpdf__container__table__d49da987'] = lib.func('void questpdf__container__table__d49da987(void* target, void* handler)');

  ContainerNativeFunctions['questpdf__container__text__357e362f'] = lib.func('void questpdf__container__text__357e362f(void* target, void* content)');

  ContainerNativeFunctions['questpdf__container__text__3f6b5b07'] = lib.func('void* questpdf__container__text__3f6b5b07(void* target, str16 text)');

  ContainerNativeFunctions['questpdf__container__translate_x__351baebe'] = lib.func('void* questpdf__container__translate_x__351baebe(void* target, float value, int32_t unit)');

  ContainerNativeFunctions['questpdf__container__translate_y__d99602db'] = lib.func('void* questpdf__container__translate_y__d99602db(void* target, float value, int32_t unit)');


ContainerNativeFunctions['questpdf__container__background_linear_gradient'] = lib.func('void* questpdf__container__background_linear_gradient(void* target, float angle, uint32_t* colors, int colorsLength)');
ContainerNativeFunctions['questpdf__container__border_linear_gradient'] = lib.func('void* questpdf__container__border_linear_gradient(void* target, float angle, uint32_t* colors, int colorsLength)');
ContainerNativeFunctions['questpdf__container__shadow'] = lib.func('void* questpdf__container__shadow(void* target, float blur, uint32_t color, float offsetX, float offsetY, float spread)');

}

// Register initialization with the global initializers
classInitializers.push((lib: koffi.IKoffiLib) => initContainerFunctions(lib));

// ============================================================================
// Container Class
// ============================================================================

export class Container {
  private readonly _ptr: NativePointer;
  private readonly _callbacks: koffi.IKoffiRegisteredCallback[] = [];

  constructor(ptr: NativePointer) {
    this._ptr = ptr;
  }

  get pointer(): NativePointer {
    return this._ptr;
  }


  
    alignLeft(): Container {


    const result = ContainerNativeFunctions['questpdf__container__align_left__68bfdc67']!(this._ptr);

    return new Container(result);

  }


  
    alignCenter(): Container {


    const result = ContainerNativeFunctions['questpdf__container__align_center__4fb1e0d1']!(this._ptr);

    return new Container(result);

  }


  
    alignRight(): Container {


    const result = ContainerNativeFunctions['questpdf__container__align_right__a1c1a1bf']!(this._ptr);

    return new Container(result);

  }


  
    alignTop(): Container {


    const result = ContainerNativeFunctions['questpdf__container__align_top__f275ca95']!(this._ptr);

    return new Container(result);

  }


  
    alignMiddle(): Container {


    const result = ContainerNativeFunctions['questpdf__container__align_middle__95fef9e8']!(this._ptr);

    return new Container(result);

  }


  
    alignBottom(): Container {


    const result = ContainerNativeFunctions['questpdf__container__align_bottom__d33d0520']!(this._ptr);

    return new Container(result);

  }


  
    column(handler: (arg0: ColumnDescriptor) => void): void {

    // Create callback type and register handler
    const handlerCallbackType_24d6ceed = koffi.proto('void handlerCb_24d6ceed(void* ptr)');
    const handlerCallbackPtr_24d6ceed = koffi.pointer(handlerCallbackType_24d6ceed);
    const handlerCb = koffi.register((ptr: NativePointer) => {
      const obj = new ColumnDescriptor(ptr);
      handler(obj);
    }, handlerCallbackPtr_24d6ceed);
    this._callbacks.push(handlerCb);


    const result = ContainerNativeFunctions['questpdf__container__column__24d6ceed']!(this._ptr, handlerCb);

  }


  
    width(value: number, unit: Unit = Unit.Point): Container {


    const result = ContainerNativeFunctions['questpdf__container__width__a346e20f']!(this._ptr, value, unit);

    return new Container(result);

  }


  
    minWidth(value: number, unit: Unit = Unit.Point): Container {


    const result = ContainerNativeFunctions['questpdf__container__min_width__c00f1915']!(this._ptr, value, unit);

    return new Container(result);

  }


  
    maxWidth(value: number, unit: Unit = Unit.Point): Container {


    const result = ContainerNativeFunctions['questpdf__container__max_width__7e85a057']!(this._ptr, value, unit);

    return new Container(result);

  }


  
    height(value: number, unit: Unit = Unit.Point): Container {


    const result = ContainerNativeFunctions['questpdf__container__height__36ac3a02']!(this._ptr, value, unit);

    return new Container(result);

  }


  
    minHeight(value: number, unit: Unit = Unit.Point): Container {


    const result = ContainerNativeFunctions['questpdf__container__min_height__58cc06b0']!(this._ptr, value, unit);

    return new Container(result);

  }


  
    maxHeight(value: number, unit: Unit = Unit.Point): Container {


    const result = ContainerNativeFunctions['questpdf__container__max_height__0b76e199']!(this._ptr, value, unit);

    return new Container(result);

  }


  
    contentFromLeftToRight(): Container {


    const result = ContainerNativeFunctions['questpdf__container__content_from_left_to_right__191523c1']!(this._ptr);

    return new Container(result);

  }


  
    contentFromRightToLeft(): Container {


    const result = ContainerNativeFunctions['questpdf__container__content_from_right_to_left__a31dbd9d']!(this._ptr);

    return new Container(result);

  }


  
    debugArea(text: string = undefined, color: Color = undefined): Container {


    const result = ContainerNativeFunctions['questpdf__container__debug_area__a69b9c65']!(this._ptr, text, color.hex);

    return new Container(result);

  }


  
    debugPointer(label: string): Container {


    const result = ContainerNativeFunctions['questpdf__container__debug_pointer__9d669879']!(this._ptr, label);

    return new Container(result);

  }


  
    decoration(handler: (arg0: DecorationDescriptor) => void): void {

    // Create callback type and register handler
    const handlerCallbackType_0b39c58e = koffi.proto('void handlerCb_0b39c58e(void* ptr)');
    const handlerCallbackPtr_0b39c58e = koffi.pointer(handlerCallbackType_0b39c58e);
    const handlerCb = koffi.register((ptr: NativePointer) => {
      const obj = new DecorationDescriptor(ptr);
      handler(obj);
    }, handlerCallbackPtr_0b39c58e);
    this._callbacks.push(handlerCb);


    const result = ContainerNativeFunctions['questpdf__container__decoration__0b39c58e']!(this._ptr, handlerCb);

  }


  
    aspectRatio(ratio: number, option: AspectRatioOption = AspectRatioOption.FitWidth): Container {


    const result = ContainerNativeFunctions['questpdf__container__aspect_ratio__fd5bc0dc']!(this._ptr, ratio, option);

    return new Container(result);

  }


  
    placeholder(text: string = undefined): void {


    const result = ContainerNativeFunctions['questpdf__container__placeholder__a560192f']!(this._ptr, text);

  }


  
    showOnce(): Container {


    const result = ContainerNativeFunctions['questpdf__container__show_once__c6224013']!(this._ptr);

    return new Container(result);

  }


  
    skipOnce(): Container {


    const result = ContainerNativeFunctions['questpdf__container__skip_once__b3d4c7bf']!(this._ptr);

    return new Container(result);

  }


  
    showEntire(): Container {


    const result = ContainerNativeFunctions['questpdf__container__show_entire__16629c88']!(this._ptr);

    return new Container(result);

  }


  
    ensureSpace(minHeight: number = 150): Container {


    const result = ContainerNativeFunctions['questpdf__container__ensure_space__0cbedd6a']!(this._ptr, minHeight);

    return new Container(result);

  }


  
    preventPageBreak(): Container {


    const result = ContainerNativeFunctions['questpdf__container__prevent_page_break__2e3cab6a']!(this._ptr);

    return new Container(result);

  }


  
    pageBreak(): void {


    const result = ContainerNativeFunctions['questpdf__container__page_break__4287fb55']!(this._ptr);

  }


  
    container(): Container {


    const result = ContainerNativeFunctions['questpdf__container__container__be126adc']!(this._ptr);

    return new Container(result);

  }


  
    hyperlink(url: string): Container {


    const result = ContainerNativeFunctions['questpdf__container__hyperlink__40aee55c']!(this._ptr, url);

    return new Container(result);

  }


  
    section(sectionName: string): Container {


    const result = ContainerNativeFunctions['questpdf__container__section__b2687119']!(this._ptr, sectionName);

    return new Container(result);

  }


  
    sectionLink(sectionName: string): Container {


    const result = ContainerNativeFunctions['questpdf__container__section_link__d27b4828']!(this._ptr, sectionName);

    return new Container(result);

  }


  
    showIf(condition: boolean): Container {


    const result = ContainerNativeFunctions['questpdf__container__show_if__da52e306']!(this._ptr, condition);

    return new Container(result);

  }


  
    unconstrained(): Container {


    const result = ContainerNativeFunctions['questpdf__container__unconstrained__a43107f6']!(this._ptr);

    return new Container(result);

  }


  
    stopPaging(): Container {


    const result = ContainerNativeFunctions['questpdf__container__stop_paging__81b05f34']!(this._ptr);

    return new Container(result);

  }


  
    scaleToFit(): Container {


    const result = ContainerNativeFunctions['questpdf__container__scale_to_fit__bb0b4e57']!(this._ptr);

    return new Container(result);

  }


  
    repeat(): Container {


    const result = ContainerNativeFunctions['questpdf__container__repeat__e198bc84']!(this._ptr);

    return new Container(result);

  }


  
    lazy(contentBuilder: (arg0: Container) => void): void {

    // Create callback type and register handler
    const contentBuilderCallbackType_971e7b54 = koffi.proto('void contentBuilderCb_971e7b54(void* ptr)');
    const contentBuilderCallbackPtr_971e7b54 = koffi.pointer(contentBuilderCallbackType_971e7b54);
    const contentBuilderCb = koffi.register((ptr: NativePointer) => {
      const obj = new Container(ptr);
      contentBuilder(obj);
    }, contentBuilderCallbackPtr_971e7b54);
    this._callbacks.push(contentBuilderCb);


    const result = ContainerNativeFunctions['questpdf__container__lazy__971e7b54']!(this._ptr, contentBuilderCb);

  }


  
    lazyWithCache(contentBuilder: (arg0: Container) => void): void {

    // Create callback type and register handler
    const contentBuilderCallbackType_a33b5f9b = koffi.proto('void contentBuilderCb_a33b5f9b(void* ptr)');
    const contentBuilderCallbackPtr_a33b5f9b = koffi.pointer(contentBuilderCallbackType_a33b5f9b);
    const contentBuilderCb = koffi.register((ptr: NativePointer) => {
      const obj = new Container(ptr);
      contentBuilder(obj);
    }, contentBuilderCallbackPtr_a33b5f9b);
    this._callbacks.push(contentBuilderCb);


    const result = ContainerNativeFunctions['questpdf__container__lazy_with_cache__a33b5f9b']!(this._ptr, contentBuilderCb);

  }


  
    zIndex(indexValue: number): Container {


    const result = ContainerNativeFunctions['questpdf__container__z_index__9cd9a32e']!(this._ptr, indexValue);

    return new Container(result);

  }


  
    captureContentPosition(id: string): Container {


    const result = ContainerNativeFunctions['questpdf__container__capture_content_position__845fb313']!(this._ptr, id);

    return new Container(result);

  }


  
    extend(): Container {


    const result = ContainerNativeFunctions['questpdf__container__extend__291e835a']!(this._ptr);

    return new Container(result);

  }


  
    extendVertical(): Container {


    const result = ContainerNativeFunctions['questpdf__container__extend_vertical__e63e1d72']!(this._ptr);

    return new Container(result);

  }


  
    extendHorizontal(): Container {


    const result = ContainerNativeFunctions['questpdf__container__extend_horizontal__c6d6d128']!(this._ptr);

    return new Container(result);

  }


  
    private image_String(filePath: string): ImageDescriptor {


    const result = ContainerNativeFunctions['questpdf__container__image__9155d14a']!(this._ptr, filePath);

    return new ImageDescriptor(result);

  }


  
    private image_Image(image: Image): ImageDescriptor {


    const result = ContainerNativeFunctions['questpdf__container__image__ccf976d1']!(this._ptr, image.pointer);

    return new ImageDescriptor(result);

  }


  
    inlined(handler: (arg0: InlinedDescriptor) => void): void {

    // Create callback type and register handler
    const handlerCallbackType_33b27c8d = koffi.proto('void handlerCb_33b27c8d(void* ptr)');
    const handlerCallbackPtr_33b27c8d = koffi.pointer(handlerCallbackType_33b27c8d);
    const handlerCb = koffi.register((ptr: NativePointer) => {
      const obj = new InlinedDescriptor(ptr);
      handler(obj);
    }, handlerCallbackPtr_33b27c8d);
    this._callbacks.push(handlerCb);


    const result = ContainerNativeFunctions['questpdf__container__inlined__33b27c8d']!(this._ptr, handlerCb);

  }


  
    layers(handler: (arg0: LayersDescriptor) => void): void {

    // Create callback type and register handler
    const handlerCallbackType_03ce5bdd = koffi.proto('void handlerCb_03ce5bdd(void* ptr)');
    const handlerCallbackPtr_03ce5bdd = koffi.pointer(handlerCallbackType_03ce5bdd);
    const handlerCb = koffi.register((ptr: NativePointer) => {
      const obj = new LayersDescriptor(ptr);
      handler(obj);
    }, handlerCallbackPtr_03ce5bdd);
    this._callbacks.push(handlerCb);


    const result = ContainerNativeFunctions['questpdf__container__layers__03ce5bdd']!(this._ptr, handlerCb);

  }


  
    lineVertical(thickness: number, unit: Unit = Unit.Point): LineDescriptor {


    const result = ContainerNativeFunctions['questpdf__container__line_vertical__ab97b857']!(this._ptr, thickness, unit);

    return new LineDescriptor(result);

  }


  
    lineHorizontal(thickness: number, unit: Unit = Unit.Point): LineDescriptor {


    const result = ContainerNativeFunctions['questpdf__container__line_horizontal__a6f7f11f']!(this._ptr, thickness, unit);

    return new LineDescriptor(result);

  }


  
    multiColumn(handler: (arg0: MultiColumnDescriptor) => void): void {

    // Create callback type and register handler
    const handlerCallbackType_193479d6 = koffi.proto('void handlerCb_193479d6(void* ptr)');
    const handlerCallbackPtr_193479d6 = koffi.pointer(handlerCallbackType_193479d6);
    const handlerCb = koffi.register((ptr: NativePointer) => {
      const obj = new MultiColumnDescriptor(ptr);
      handler(obj);
    }, handlerCallbackPtr_193479d6);
    this._callbacks.push(handlerCb);


    const result = ContainerNativeFunctions['questpdf__container__multi_column__193479d6']!(this._ptr, handlerCb);

  }


  
    padding(value: number, unit: Unit = Unit.Point): Container {


    const result = ContainerNativeFunctions['questpdf__container__padding__5daecb7e']!(this._ptr, value, unit);

    return new Container(result);

  }


  
    paddingHorizontal(value: number, unit: Unit = Unit.Point): Container {


    const result = ContainerNativeFunctions['questpdf__container__padding_horizontal__7a6b255d']!(this._ptr, value, unit);

    return new Container(result);

  }


  
    paddingVertical(value: number, unit: Unit = Unit.Point): Container {


    const result = ContainerNativeFunctions['questpdf__container__padding_vertical__91122aaa']!(this._ptr, value, unit);

    return new Container(result);

  }


  
    paddingTop(value: number, unit: Unit = Unit.Point): Container {


    const result = ContainerNativeFunctions['questpdf__container__padding_top__de3b7b3b']!(this._ptr, value, unit);

    return new Container(result);

  }


  
    paddingBottom(value: number, unit: Unit = Unit.Point): Container {


    const result = ContainerNativeFunctions['questpdf__container__padding_bottom__74ad0a7b']!(this._ptr, value, unit);

    return new Container(result);

  }


  
    paddingLeft(value: number, unit: Unit = Unit.Point): Container {


    const result = ContainerNativeFunctions['questpdf__container__padding_left__103ee738']!(this._ptr, value, unit);

    return new Container(result);

  }


  
    paddingRight(value: number, unit: Unit = Unit.Point): Container {


    const result = ContainerNativeFunctions['questpdf__container__padding_right__89d1cf61']!(this._ptr, value, unit);

    return new Container(result);

  }


  
    rotateLeft(): Container {


    const result = ContainerNativeFunctions['questpdf__container__rotate_left__c5193e66']!(this._ptr);

    return new Container(result);

  }


  
    rotateRight(): Container {


    const result = ContainerNativeFunctions['questpdf__container__rotate_right__004c9c52']!(this._ptr);

    return new Container(result);

  }


  
    rotate(angle: number): Container {


    const result = ContainerNativeFunctions['questpdf__container__rotate__c33f62ac']!(this._ptr, angle);

    return new Container(result);

  }


  
    row(handler: (arg0: RowDescriptor) => void): void {

    // Create callback type and register handler
    const handlerCallbackType_39fce557 = koffi.proto('void handlerCb_39fce557(void* ptr)');
    const handlerCallbackPtr_39fce557 = koffi.pointer(handlerCallbackType_39fce557);
    const handlerCb = koffi.register((ptr: NativePointer) => {
      const obj = new RowDescriptor(ptr);
      handler(obj);
    }, handlerCallbackPtr_39fce557);
    this._callbacks.push(handlerCb);


    const result = ContainerNativeFunctions['questpdf__container__row__39fce557']!(this._ptr, handlerCb);

  }


  
    scale(factor: number): Container {


    const result = ContainerNativeFunctions['questpdf__container__scale__05521931']!(this._ptr, factor);

    return new Container(result);

  }


  
    scaleHorizontal(factor: number): Container {


    const result = ContainerNativeFunctions['questpdf__container__scale_horizontal__14d1a9be']!(this._ptr, factor);

    return new Container(result);

  }


  
    scaleVertical(factor: number): Container {


    const result = ContainerNativeFunctions['questpdf__container__scale_vertical__5bc8a8a5']!(this._ptr, factor);

    return new Container(result);

  }


  
    flipHorizontal(): Container {


    const result = ContainerNativeFunctions['questpdf__container__flip_horizontal__744e4fe9']!(this._ptr);

    return new Container(result);

  }


  
    flipVertical(): Container {


    const result = ContainerNativeFunctions['questpdf__container__flip_vertical__a91487f3']!(this._ptr);

    return new Container(result);

  }


  
    flipOver(): Container {


    const result = ContainerNativeFunctions['questpdf__container__flip_over__ce1ff345']!(this._ptr);

    return new Container(result);

  }


  
    shrink(): Container {


    const result = ContainerNativeFunctions['questpdf__container__shrink__4221b85b']!(this._ptr);

    return new Container(result);

  }


  
    shrinkVertical(): Container {


    const result = ContainerNativeFunctions['questpdf__container__shrink_vertical__e5042c3c']!(this._ptr);

    return new Container(result);

  }


  
    shrinkHorizontal(): Container {


    const result = ContainerNativeFunctions['questpdf__container__shrink_horizontal__588cfd0f']!(this._ptr);

    return new Container(result);

  }


  
    private border_Float_Color(all: number, color: Color): Container {


    const result = ContainerNativeFunctions['questpdf__container__border__a6712928']!(this._ptr, all, color.hex);

    return new Container(result);

  }


  
    background(color: Color): Container {


    const result = ContainerNativeFunctions['questpdf__container__background__68f98b81']!(this._ptr, color.hex);

    return new Container(result);

  }


  
    private border_Float_Unit(value: number, unit: Unit = Unit.Point): Container {


    const result = ContainerNativeFunctions['questpdf__container__border__17f3b5e4']!(this._ptr, value, unit);

    return new Container(result);

  }


  
    borderVertical(value: number, unit: Unit = Unit.Point): Container {


    const result = ContainerNativeFunctions['questpdf__container__border_vertical__7922384b']!(this._ptr, value, unit);

    return new Container(result);

  }


  
    borderHorizontal(value: number, unit: Unit = Unit.Point): Container {


    const result = ContainerNativeFunctions['questpdf__container__border_horizontal__34913f34']!(this._ptr, value, unit);

    return new Container(result);

  }


  
    borderLeft(value: number, unit: Unit = Unit.Point): Container {


    const result = ContainerNativeFunctions['questpdf__container__border_left__803ed1e6']!(this._ptr, value, unit);

    return new Container(result);

  }


  
    borderRight(value: number, unit: Unit = Unit.Point): Container {


    const result = ContainerNativeFunctions['questpdf__container__border_right__de8ca6bf']!(this._ptr, value, unit);

    return new Container(result);

  }


  
    borderTop(value: number, unit: Unit = Unit.Point): Container {


    const result = ContainerNativeFunctions['questpdf__container__border_top__c469b91f']!(this._ptr, value, unit);

    return new Container(result);

  }


  
    borderBottom(value: number, unit: Unit = Unit.Point): Container {


    const result = ContainerNativeFunctions['questpdf__container__border_bottom__59b8a019']!(this._ptr, value, unit);

    return new Container(result);

  }


  
    cornerRadius(value: number = 0, unit: Unit = Unit.Point): Container {


    const result = ContainerNativeFunctions['questpdf__container__corner_radius__bf7cb39f']!(this._ptr, value, unit);

    return new Container(result);

  }


  
    cornerRadiusTopLeft(value: number = 0, unit: Unit = Unit.Point): Container {


    const result = ContainerNativeFunctions['questpdf__container__corner_radius_top_left__41d08c72']!(this._ptr, value, unit);

    return new Container(result);

  }


  
    cornerRadiusTopRight(value: number = 0, unit: Unit = Unit.Point): Container {


    const result = ContainerNativeFunctions['questpdf__container__corner_radius_top_right__1497678a']!(this._ptr, value, unit);

    return new Container(result);

  }


  
    cornerRadiusBottomLeft(value: number = 0, unit: Unit = Unit.Point): Container {


    const result = ContainerNativeFunctions['questpdf__container__corner_radius_bottom_left__3a8d234a']!(this._ptr, value, unit);

    return new Container(result);

  }


  
    cornerRadiusBottomRight(value: number = 0, unit: Unit = Unit.Point): Container {


    const result = ContainerNativeFunctions['questpdf__container__corner_radius_bottom_right__b07c1d8d']!(this._ptr, value, unit);

    return new Container(result);

  }


  
    borderColor(color: Color): Container {


    const result = ContainerNativeFunctions['questpdf__container__border_color__2a24bda0']!(this._ptr, color.hex);

    return new Container(result);

  }


  
    borderAlignmentOutside(): Container {


    const result = ContainerNativeFunctions['questpdf__container__border_alignment_outside__ce5e63fa']!(this._ptr);

    return new Container(result);

  }


  
    borderAlignmentMiddle(): Container {


    const result = ContainerNativeFunctions['questpdf__container__border_alignment_middle__66a27445']!(this._ptr);

    return new Container(result);

  }


  
    borderAlignmentInside(): Container {


    const result = ContainerNativeFunctions['questpdf__container__border_alignment_inside__8cef56b1']!(this._ptr);

    return new Container(result);

  }


  
    private svg_String(svg: string): SvgImageDescriptor {


    const result = ContainerNativeFunctions['questpdf__container__svg__f547d46e']!(this._ptr, svg);

    return new SvgImageDescriptor(result);

  }


  
    private svg_SvgImage(image: SvgImage): SvgImageDescriptor {


    const result = ContainerNativeFunctions['questpdf__container__svg__b1de06e3']!(this._ptr, image.pointer);

    return new SvgImageDescriptor(result);

  }


  
    table(handler: (arg0: TableDescriptor) => void): void {

    // Create callback type and register handler
    const handlerCallbackType_d49da987 = koffi.proto('void handlerCb_d49da987(void* ptr)');
    const handlerCallbackPtr_d49da987 = koffi.pointer(handlerCallbackType_d49da987);
    const handlerCb = koffi.register((ptr: NativePointer) => {
      const obj = new TableDescriptor(ptr);
      handler(obj);
    }, handlerCallbackPtr_d49da987);
    this._callbacks.push(handlerCb);


    const result = ContainerNativeFunctions['questpdf__container__table__d49da987']!(this._ptr, handlerCb);

  }


  
    private text_Action(content: (arg0: TextDescriptor) => void): void {

    // Create callback type and register handler
    const contentCallbackType_357e362f = koffi.proto('void contentCb_357e362f(void* ptr)');
    const contentCallbackPtr_357e362f = koffi.pointer(contentCallbackType_357e362f);
    const contentCb = koffi.register((ptr: NativePointer) => {
      const obj = new TextDescriptor(ptr);
      content(obj);
    }, contentCallbackPtr_357e362f);
    this._callbacks.push(contentCb);


    const result = ContainerNativeFunctions['questpdf__container__text__357e362f']!(this._ptr, contentCb);

  }


  
    private text_String(text: string): TextBlockDescriptor {


    const result = ContainerNativeFunctions['questpdf__container__text__3f6b5b07']!(this._ptr, text);

    return new TextBlockDescriptor(result);

  }


  
    translateX(value: number, unit: Unit = Unit.Point): Container {


    const result = ContainerNativeFunctions['questpdf__container__translate_x__351baebe']!(this._ptr, value, unit);

    return new Container(result);

  }


  
    translateY(value: number, unit: Unit = Unit.Point): Container {


    const result = ContainerNativeFunctions['questpdf__container__translate_y__d99602db']!(this._ptr, value, unit);

    return new Container(result);

  }



backgroundLinearGradient(angle: number, colors: Color[]): Container {
    const arr = new Uint32Array(colors.map(x => x.hex));
    const result = ContainerNativeFunctions['questpdf__container__background_linear_gradient']!(this._ptr, angle, arr, arr.length);
    return new Container(result);
}

borderLinearGradient(angle: number, colors: Color[]): Container {
    const arr = new Uint32Array(colors.map(x => x.hex));
    const result = ContainerNativeFunctions['questpdf__container__border_linear_gradient']!(this._ptr, angle, arr, arr.length);
    return new Container(result);
}

shadow({ blur = 0, color = Colors.black, offsetX = 0, offsetY = 0, spread = 0 }: { blur?: number, color?: Color, offsetX?: number, offsetY?: number, spread?: number }): Container {
    const result = ContainerNativeFunctions['questpdf__container__shadow']!(this._ptr, blur, color.hex, offsetX, offsetY, spread);
    return new Container(result);
}

}


// ============================================================================
// TableCellContainer - Native Function Declarations
// ============================================================================





const TableCellContainerNativeFunctions: Record<string, ((...args: any[]) => any) | null> = {};

function initTableCellContainerFunctions(lib: koffi.IKoffiLib): void {

  TableCellContainerNativeFunctions['questpdf__table_cell_container__column__384372f0'] = lib.func('void* questpdf__table_cell_container__column__384372f0(void* target, uint32_t value)');

  TableCellContainerNativeFunctions['questpdf__table_cell_container__column_span__629b3552'] = lib.func('void* questpdf__table_cell_container__column_span__629b3552(void* target, uint32_t value)');

  TableCellContainerNativeFunctions['questpdf__table_cell_container__row__7ddb9999'] = lib.func('void* questpdf__table_cell_container__row__7ddb9999(void* target, uint32_t value)');

  TableCellContainerNativeFunctions['questpdf__table_cell_container__row_span__e9016d30'] = lib.func('void* questpdf__table_cell_container__row_span__e9016d30(void* target, uint32_t value)');



}

// Register initialization with the global initializers
classInitializers.push((lib: koffi.IKoffiLib) => initTableCellContainerFunctions(lib));

// ============================================================================
// TableCellContainer Class
// ============================================================================

export class TableCellContainer {
  private readonly _ptr: NativePointer;
  private readonly _callbacks: koffi.IKoffiRegisteredCallback[] = [];

  constructor(ptr: NativePointer) {
    this._ptr = ptr;
  }

  get pointer(): NativePointer {
    return this._ptr;
  }


  
    column(value: number): TableCellContainer {


    const result = TableCellContainerNativeFunctions['questpdf__table_cell_container__column__384372f0']!(this._ptr, value);

    return new TableCellContainer(result);

  }


  
    columnSpan(value: number): TableCellContainer {


    const result = TableCellContainerNativeFunctions['questpdf__table_cell_container__column_span__629b3552']!(this._ptr, value);

    return new TableCellContainer(result);

  }


  
    row(value: number): TableCellContainer {


    const result = TableCellContainerNativeFunctions['questpdf__table_cell_container__row__7ddb9999']!(this._ptr, value);

    return new TableCellContainer(result);

  }


  
    rowSpan(value: number): TableCellContainer {


    const result = TableCellContainerNativeFunctions['questpdf__table_cell_container__row_span__e9016d30']!(this._ptr, value);

    return new TableCellContainer(result);

  }




}



// ============================================================================
// Native Function Declarations (Manual)
// ============================================================================

interface QuestPDFLib {
  questpdf_sum(a: number, b: number): number;
  questpdf_document_create(cb: koffi.IKoffiRegisteredCallback): NativePointer;
  questpdf_document_generate_pdf(document: NativePointer): { data: NativePointer; length: number };
  questpdf_document_destroy(document: NativePointer): void;
  questpdf_document_container_add_page(container: NativePointer, cb: koffi.IKoffiRegisteredCallback): void;
  questpdf_page_set_margin(page: NativePointer, margin: number): void;
  questpdf_page_set_content(page: NativePointer): NativePointer;
  questpdf_container_background(container: NativePointer, color: number): NativePointer;
  questpdf_free_bytes(ptr: NativePointer): void;
  [key: string]: (...args: any[]) => any;
}

let questpdfLib: QuestPDFLib | null = null;

// ============================================================================
// Library Initialization
// ============================================================================

export function initializeQuestPDF(libraryPath?: string): void {
  if (lib !== null) {
    return; // Already initialized
  }

  const libPath = libraryPath ?? getLibraryPath();
  lib = koffi.load(libPath);

  // Define manual functions
  // Note: Callback parameters use void* in the signature - koffi handles the conversion
  questpdfLib = {
    questpdf_sum: lib.func('int questpdf_sum(int a, int b)'),
    questpdf_document_create: lib.func('void* questpdf_document_create(void* cb)'),
    questpdf_document_generate_pdf: lib.func('Buffer questpdf_document_generate_pdf(void* document)'),
    questpdf_document_destroy: lib.func('void questpdf_document_destroy(void* document)'),
    questpdf_document_container_add_page: lib.func('void questpdf_document_container_add_page(void* container, void* cb)'),
    questpdf_page_set_margin: lib.func('void questpdf_page_set_margin(void* page, int margin)'),
    questpdf_page_set_content: lib.func('void* questpdf_page_set_content(void* page)'),
    questpdf_container_background: lib.func('void* questpdf_container_background(void* container, uint32_t color)'),
    questpdf_free_bytes: lib.func('void questpdf_free_bytes(void* ptr)'),
  };

  // Initialize all generated class functions
  for (const initializer of classInitializers) {
    initializer(lib);
  }
}

export function getLib(): QuestPDFLib {
  if (questpdfLib === null) {
    throw new Error('QuestPDF library not initialized. Call initializeQuestPDF() first.');
  }
  return questpdfLib;
}

export function getNativeLib(): koffi.IKoffiLib {
  if (lib === null) {
    throw new Error('QuestPDF library not initialized. Call initializeQuestPDF() first.');
  }
  return lib;
}

// ============================================================================
// Helper Classes
// ============================================================================

export class DocumentContainer {
  private readonly _ptr: NativePointer;
  private readonly _callbacks: koffi.IKoffiRegisteredCallback[] = [];

  constructor(ptr: NativePointer) {
    this._ptr = ptr;
  }

  get pointer(): NativePointer {
    return this._ptr;
  }

  page(configurator: (page: PageDescriptor) => void): void {
    const callback = koffi.register((pagePtr: NativePointer) => {
      const page = new PageDescriptor(pagePtr);
      configurator(page);
    }, PageDescriptorCallbackPtr);

    this._callbacks.push(callback);
    getLib().questpdf_document_container_add_page(this._ptr, callback);
  }
}

export class Document {
  private _ptr: NativePointer | null = null;
  private _containerCallback: koffi.IKoffiRegisteredCallback | null = null;

  private constructor() {}

  static create(configurator: (container: DocumentContainer) => void): Document {
    const doc = new Document();

    doc._containerCallback = koffi.register((containerPtr: NativePointer) => {
      const container = new DocumentContainer(containerPtr);
      configurator(container);
    }, DocumentContainerCallbackPtr);

    doc._ptr = getLib().questpdf_document_create(doc._containerCallback);
    return doc;
  }

  generatePdf(): Buffer {
    if (this._ptr === null) {
      throw new Error('Document not created');
    }

    const result = getLib().questpdf_document_generate_pdf(this._ptr);
    try {
      // Copy data to a Node.js Buffer
      const data = koffi.decode(result.data as NativePointer, 'uint8_t', result.length);
      return Buffer.from(data as ArrayLike<number>);
    } finally {
      getLib().questpdf_free_bytes(result.data);
    }
  }

  saveToFile(filename: string): Document {
    const pdfBytes = this.generatePdf();
    fs.writeFileSync(filename, pdfBytes);
    return this;
  }

  destroy(): void {
    if (this._ptr !== null) {
      getLib().questpdf_document_destroy(this._ptr);
      this._ptr = null;
    }
    if (this._containerCallback !== null) {
      koffi.unregister(this._containerCallback);
      this._containerCallback = null;
    }
  }
}

// ============================================================================
// Exports
// ============================================================================

export { koffi, NativePointer };
