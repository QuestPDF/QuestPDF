# AUTO-GENERATED on 02/23/2026 17:01:08


import os
from cffi import FFI
from typing import Callable, Optional, Tuple, Any, Self, Protocol, overload, Union

from dataclasses import dataclass

import warnings
import functools

try:
    from warnings import deprecated
except ImportError:
    def deprecated(reason: str):
        def decorator(func):
            @functools.wraps(func)
            def wrapper(*args, **kwargs):
                warnings.warn(
                    reason,
                    DeprecationWarning,
                    stacklevel=2
                )
                return func(*args, **kwargs)

            return wrapper

        return decorator
    

questpdf_ffi = FFI()

QUESTPDF_CDEF = r"""
    typedef unsigned char      uint8_t;
    typedef signed char        int8_t;
    typedef unsigned short     uint16_t;
    typedef short              int16_t;
    typedef unsigned int       uint32_t;
    typedef int                int32_t;
    typedef unsigned long long uint64_t;
    typedef long long          int64_t;
    
"""

class QuestPDF:
    def __init__(self):
        self.lib = None

questpdf = QuestPDF()


def decode_text_as_utf_8(value) -> str:
    return questpdf_ffi.string(value).decode("utf-8")


def decode_byte_array(value) -> str:
    return questpdf_ffi.string(value)


class Size:
    None

class ImageSize:
    None


QUESTPDF_CDEF += r"""
    typedef struct Buffer {
        uint8_t* data;                // or: unsigned char* data;
        size_t   length;
    } Buffer;
"""


class Color:
    def __init__(self, hex_val: int):
        # Ensure it stays within 32-bit unsigned limit
        self.hex = hex_val & 0xFFFFFFFF

    @property
    def alpha(self) -> int:
        return (self.hex >> 24) & 0xFF

    @property
    def red(self) -> int:
        return (self.hex >> 16) & 0xFF

    @property
    def green(self) -> int:
        return (self.hex >> 8) & 0xFF

    @property
    def blue(self) -> int:
        return self.hex & 0xFF

    def __str__(self):
        if self.alpha == 0xFF:
            return f"#{self.red:02X}{self.green:02X}{self.blue:02X}"

        return f"#{self.alpha:02X}{self.red:02X}{self.green:02X}{self.blue:02X}"

    def __repr__(self):
        return self.__str__()
        

@dataclass(frozen=True)
class Colors:

    black = Color(0xFF000000)

    white = Color(0xFFFFFFFF)

    transparent = Color(0x00000000)



    @dataclass(frozen=True)
    class red:

        lighten_5 = Color(0xFFFFEBEE)

        lighten_4 = Color(0xFFFFCDD2)

        lighten_3 = Color(0xFFEF9A9A)

        lighten_2 = Color(0xFFE57373)

        lighten_1 = Color(0xFFEF5350)

        medium = Color(0xFFF44336)

        darken_1 = Color(0xFFE53935)

        darken_2 = Color(0xFFD32F2F)

        darken_3 = Color(0xFFC62828)

        darken_4 = Color(0xFFB71C1C)

        accent_1 = Color(0xFFFF8A80)

        accent_2 = Color(0xFFFF5252)

        accent_3 = Color(0xFFFF1744)

        accent_4 = Color(0xFFD50000)


    @dataclass(frozen=True)
    class pink:

        lighten_5 = Color(0xFFFCE4EC)

        lighten_4 = Color(0xFFF8BBD0)

        lighten_3 = Color(0xFFF48FB1)

        lighten_2 = Color(0xFFF06292)

        lighten_1 = Color(0xFFEC407A)

        medium = Color(0xFFE91E63)

        darken_1 = Color(0xFFD81B60)

        darken_2 = Color(0xFFC2185B)

        darken_3 = Color(0xFFAD1457)

        darken_4 = Color(0xFF880E4F)

        accent_1 = Color(0xFFFF80AB)

        accent_2 = Color(0xFFFF4081)

        accent_3 = Color(0xFFF50057)

        accent_4 = Color(0xFFC51162)


    @dataclass(frozen=True)
    class purple:

        lighten_5 = Color(0xFFF3E5F5)

        lighten_4 = Color(0xFFE1BEE7)

        lighten_3 = Color(0xFFCE93D8)

        lighten_2 = Color(0xFFBA68C8)

        lighten_1 = Color(0xFFAB47BC)

        medium = Color(0xFF9C27B0)

        darken_1 = Color(0xFF8E24AA)

        darken_2 = Color(0xFF7B1FA2)

        darken_3 = Color(0xFF6A1B9A)

        darken_4 = Color(0xFF4A148C)

        accent_1 = Color(0xFFEA80FC)

        accent_2 = Color(0xFFE040FB)

        accent_3 = Color(0xFFD500F9)

        accent_4 = Color(0xFFAA00FF)


    @dataclass(frozen=True)
    class deep_purple:

        lighten_5 = Color(0xFFEDE7F6)

        lighten_4 = Color(0xFFD1C4E9)

        lighten_3 = Color(0xFFB39DDB)

        lighten_2 = Color(0xFF9575CD)

        lighten_1 = Color(0xFF7E57C2)

        medium = Color(0xFF673AB7)

        darken_1 = Color(0xFF5E35B1)

        darken_2 = Color(0xFF512DA8)

        darken_3 = Color(0xFF4527A0)

        darken_4 = Color(0xFF311B92)

        accent_1 = Color(0xFFB388FF)

        accent_2 = Color(0xFF7C4DFF)

        accent_3 = Color(0xFF651FFF)

        accent_4 = Color(0xFF6200EA)


    @dataclass(frozen=True)
    class indigo:

        lighten_5 = Color(0xFFE8EAF6)

        lighten_4 = Color(0xFFC5CAE9)

        lighten_3 = Color(0xFF9FA8DA)

        lighten_2 = Color(0xFF7986CB)

        lighten_1 = Color(0xFF5C6BC0)

        medium = Color(0xFF3F51B5)

        darken_1 = Color(0xFF3949AB)

        darken_2 = Color(0xFF303F9F)

        darken_3 = Color(0xFF283593)

        darken_4 = Color(0xFF1A237E)

        accent_1 = Color(0xFF8C9EFF)

        accent_2 = Color(0xFF536DFE)

        accent_3 = Color(0xFF3D5AFE)

        accent_4 = Color(0xFF304FFE)


    @dataclass(frozen=True)
    class blue:

        lighten_5 = Color(0xFFE3F2FD)

        lighten_4 = Color(0xFFBBDEFB)

        lighten_3 = Color(0xFF90CAF9)

        lighten_2 = Color(0xFF64B5F6)

        lighten_1 = Color(0xFF42A5F5)

        medium = Color(0xFF2196F3)

        darken_1 = Color(0xFF1E88E5)

        darken_2 = Color(0xFF1976D2)

        darken_3 = Color(0xFF1565C0)

        darken_4 = Color(0xFF0D47A1)

        accent_1 = Color(0xFF82B1FF)

        accent_2 = Color(0xFF448AFF)

        accent_3 = Color(0xFF2979FF)

        accent_4 = Color(0xFF2962FF)


    @dataclass(frozen=True)
    class light_blue:

        lighten_5 = Color(0xFFE1F5FE)

        lighten_4 = Color(0xFFB3E5FC)

        lighten_3 = Color(0xFF81D4FA)

        lighten_2 = Color(0xFF4FC3F7)

        lighten_1 = Color(0xFF29B6F6)

        medium = Color(0xFF03A9F4)

        darken_1 = Color(0xFF039BE5)

        darken_2 = Color(0xFF0288D1)

        darken_3 = Color(0xFF0277BD)

        darken_4 = Color(0xFF01579B)

        accent_1 = Color(0xFF80D8FF)

        accent_2 = Color(0xFF40C4FF)

        accent_3 = Color(0xFF00B0FF)

        accent_4 = Color(0xFF0091EA)


    @dataclass(frozen=True)
    class cyan:

        lighten_5 = Color(0xFFE0F7FA)

        lighten_4 = Color(0xFFB2EBF2)

        lighten_3 = Color(0xFF80DEEA)

        lighten_2 = Color(0xFF4DD0E1)

        lighten_1 = Color(0xFF26C6DA)

        medium = Color(0xFF00BCD4)

        darken_1 = Color(0xFF00ACC1)

        darken_2 = Color(0xFF0097A7)

        darken_3 = Color(0xFF00838F)

        darken_4 = Color(0xFF006064)

        accent_1 = Color(0xFF84FFFF)

        accent_2 = Color(0xFF18FFFF)

        accent_3 = Color(0xFF00E5FF)

        accent_4 = Color(0xFF00B8D4)


    @dataclass(frozen=True)
    class teal:

        lighten_5 = Color(0xFFE0F2F1)

        lighten_4 = Color(0xFFB2DFDB)

        lighten_3 = Color(0xFF80CBC4)

        lighten_2 = Color(0xFF4DB6AC)

        lighten_1 = Color(0xFF26A69A)

        medium = Color(0xFF009688)

        darken_1 = Color(0xFF00897B)

        darken_2 = Color(0xFF00796B)

        darken_3 = Color(0xFF00695C)

        darken_4 = Color(0xFF004D40)

        accent_1 = Color(0xFFA7FFEB)

        accent_2 = Color(0xFF64FFDA)

        accent_3 = Color(0xFF1DE9B6)

        accent_4 = Color(0xFF00BFA5)


    @dataclass(frozen=True)
    class green:

        lighten_5 = Color(0xFFE8F5E9)

        lighten_4 = Color(0xFFC8E6C9)

        lighten_3 = Color(0xFFA5D6A7)

        lighten_2 = Color(0xFF81C784)

        lighten_1 = Color(0xFF66BB6A)

        medium = Color(0xFF4CAF50)

        darken_1 = Color(0xFF43A047)

        darken_2 = Color(0xFF388E3C)

        darken_3 = Color(0xFF2E7D32)

        darken_4 = Color(0xFF1B5E20)

        accent_1 = Color(0xFFB9F6CA)

        accent_2 = Color(0xFF69F0AE)

        accent_3 = Color(0xFF00E676)

        accent_4 = Color(0xFF00C853)


    @dataclass(frozen=True)
    class light_green:

        lighten_5 = Color(0xFFF1F8E9)

        lighten_4 = Color(0xFFDCEDC8)

        lighten_3 = Color(0xFFC5E1A5)

        lighten_2 = Color(0xFFAED581)

        lighten_1 = Color(0xFF9CCC65)

        medium = Color(0xFF8BC34A)

        darken_1 = Color(0xFF7CB342)

        darken_2 = Color(0xFF689F38)

        darken_3 = Color(0xFF558B2F)

        darken_4 = Color(0xFF33691E)

        accent_1 = Color(0xFFCCFF90)

        accent_2 = Color(0xFFB2FF59)

        accent_3 = Color(0xFF76FF03)

        accent_4 = Color(0xFF64DD17)


    @dataclass(frozen=True)
    class lime:

        lighten_5 = Color(0xFFF9FBE7)

        lighten_4 = Color(0xFFF0F4C3)

        lighten_3 = Color(0xFFE6EE9C)

        lighten_2 = Color(0xFFDCE775)

        lighten_1 = Color(0xFFD4E157)

        medium = Color(0xFFCDDC39)

        darken_1 = Color(0xFFC0CA33)

        darken_2 = Color(0xFFAFB42B)

        darken_3 = Color(0xFF9E9D24)

        darken_4 = Color(0xFF827717)

        accent_1 = Color(0xFFF4FF81)

        accent_2 = Color(0xFFEEFF41)

        accent_3 = Color(0xFFC6FF00)

        accent_4 = Color(0xFFAEEA00)


    @dataclass(frozen=True)
    class yellow:

        lighten_5 = Color(0xFFFFFDE7)

        lighten_4 = Color(0xFFFFF9C4)

        lighten_3 = Color(0xFFFFF59D)

        lighten_2 = Color(0xFFFFF176)

        lighten_1 = Color(0xFFFFEE58)

        medium = Color(0xFFFFEB3B)

        darken_1 = Color(0xFFFDD835)

        darken_2 = Color(0xFFFBC02D)

        darken_3 = Color(0xFFF9A825)

        darken_4 = Color(0xFFF57F17)

        accent_1 = Color(0xFFFFFF8D)

        accent_2 = Color(0xFFFFFF00)

        accent_3 = Color(0xFFFFEA00)

        accent_4 = Color(0xFFFFD600)


    @dataclass(frozen=True)
    class amber:

        lighten_5 = Color(0xFFFFF8E1)

        lighten_4 = Color(0xFFFFECB3)

        lighten_3 = Color(0xFFFFE082)

        lighten_2 = Color(0xFFFFD54F)

        lighten_1 = Color(0xFFFFCA28)

        medium = Color(0xFFFFC107)

        darken_1 = Color(0xFFFFB300)

        darken_2 = Color(0xFFFFA000)

        darken_3 = Color(0xFFFF8F00)

        darken_4 = Color(0xFFFF6F00)

        accent_1 = Color(0xFFFFE57F)

        accent_2 = Color(0xFFFFD740)

        accent_3 = Color(0xFFFFC400)

        accent_4 = Color(0xFFFFAB00)


    @dataclass(frozen=True)
    class orange:

        lighten_5 = Color(0xFFFFF3E0)

        lighten_4 = Color(0xFFFFE0B2)

        lighten_3 = Color(0xFFFFCC80)

        lighten_2 = Color(0xFFFFB74D)

        lighten_1 = Color(0xFFFFA726)

        medium = Color(0xFFFF9800)

        darken_1 = Color(0xFFFB8C00)

        darken_2 = Color(0xFFF57C00)

        darken_3 = Color(0xFFEF6C00)

        darken_4 = Color(0xFFE65100)

        accent_1 = Color(0xFFFFD180)

        accent_2 = Color(0xFFFFAB40)

        accent_3 = Color(0xFFFF9100)

        accent_4 = Color(0xFFFF6D00)


    @dataclass(frozen=True)
    class deep_orange:

        lighten_5 = Color(0xFFFBE9E7)

        lighten_4 = Color(0xFFFFCCBC)

        lighten_3 = Color(0xFFFFAB91)

        lighten_2 = Color(0xFFFF8A65)

        lighten_1 = Color(0xFFFF7043)

        medium = Color(0xFFFF5722)

        darken_1 = Color(0xFFF4511E)

        darken_2 = Color(0xFFE64A19)

        darken_3 = Color(0xFFD84315)

        darken_4 = Color(0xFFBF360C)

        accent_1 = Color(0xFFFF9E80)

        accent_2 = Color(0xFFFF6E40)

        accent_3 = Color(0xFFFF3D00)

        accent_4 = Color(0xFFDD2C00)


    @dataclass(frozen=True)
    class brown:

        lighten_5 = Color(0xFFEFEBE9)

        lighten_4 = Color(0xFFD7CCC8)

        lighten_3 = Color(0xFFBCAAA4)

        lighten_2 = Color(0xFFA1887F)

        lighten_1 = Color(0xFF8D6E63)

        medium = Color(0xFF795548)

        darken_1 = Color(0xFF6D4C41)

        darken_2 = Color(0xFF5D4037)

        darken_3 = Color(0xFF4E342E)

        darken_4 = Color(0xFF3E2723)


    @dataclass(frozen=True)
    class grey:

        lighten_5 = Color(0xFFFAFAFA)

        lighten_4 = Color(0xFFF5F5F5)

        lighten_3 = Color(0xFFEEEEEE)

        lighten_2 = Color(0xFFE0E0E0)

        lighten_1 = Color(0xFFBDBDBD)

        medium = Color(0xFF9E9E9E)

        darken_1 = Color(0xFF757575)

        darken_2 = Color(0xFF616161)

        darken_3 = Color(0xFF424242)

        darken_4 = Color(0xFF212121)


    @dataclass(frozen=True)
    class blue_grey:

        lighten_5 = Color(0xFFECEFF1)

        lighten_4 = Color(0xFFCFD8DC)

        lighten_3 = Color(0xFFB0BEC5)

        lighten_2 = Color(0xFF90A4AE)

        lighten_1 = Color(0xFF78909C)

        medium = Color(0xFF607D8B)

        darken_1 = Color(0xFF546E7A)

        darken_2 = Color(0xFF455A64)

        darken_3 = Color(0xFF37474F)

        darken_4 = Color(0xFF263238)




from enum import Enum


class AspectRatioOption(Enum):

    fit_width = 0

    fit_height = 1

    fit_area = 2


class ContentDirection(Enum):

    left_to_right = 0

    right_to_left = 1


class FontWeight(Enum):

    thin = 100

    extra_light = 200

    light = 300

    normal = 400

    medium = 500

    semi_bold = 600

    bold = 700

    extra_bold = 800

    black = 900

    extra_black = 1000


class ImageCompressionQuality(Enum):

    best = 0

    very_high = 1

    high = 2

    medium = 3

    low = 4

    very_low = 5


class ImageFormat(Enum):

    jpeg = 0

    png = 1

    webp = 2


class ImageScaling(Enum):

    fit_width = 0

    fit_height = 1

    fit_area = 2

    resize = 3


class LicenseType(Enum):

    community = 0

    professional = 1

    enterprise = 2


class TextHorizontalAlignment(Enum):

    left = 0

    center = 1

    right = 2

    justify = 3

    start = 4

    end = 5


class TextInjectedElementAlignment(Enum):

    above_baseline = 0

    below_baseline = 1

    top = 2

    bottom = 3

    middle = 4


class Unit(Enum):

    point = 0

    meter = 1

    centimetre = 2

    millimetre = 3

    feet = 4

    inch = 5

    mil = 6



QUESTPDF_CDEF += r"""
    void questpdf__settings__license(int value);
    void questpdf__settings__enable_debugging(bool value);
    void questpdf__settings__enable_caching(bool value);
    void questpdf__settings__check_if_all_text_glyphs_are_available(bool value);
    void questpdf__settings__use_environment_fonts(bool value);
    void questpdf__settings__temporary_storage_path(const char* value);
    void questpdf__settings__font_discovery_paths(const char* delimited_list);
"""


class Settings:
    @classmethod
    def set_license(cls, value: LicenseType | None) -> None:
        if value is None:
            return

        questpdf.lib.questpdf__settings__license(value.value)

    @classmethod
    def set_enable_debugging(cls, value: bool) -> None:
        questpdf.lib.questpdf__settings__enable_debugging(value)

    @classmethod
    def set_enable_caching(cls, value: bool) -> None:
        questpdf.lib.questpdf__settings__enable_caching(value)

    @classmethod
    def set_check_if_all_text_glyphs_are_available(cls, value: bool) -> None:
        questpdf.lib.questpdf__settings__check_if_all_text_glyphs_are_available(value)

    @classmethod
    def set_use_environment_fonts(cls, value: bool) -> None:
        questpdf.lib.questpdf__settings__use_environment_fonts(value)

    @classmethod
    def set_temporary_storage_path(cls, value: str) -> None:
        questpdf.lib.questpdf__settings__temporary_storage_path(value.encode('utf-8'))

    @classmethod
    def set_font_discovery_paths(cls, value: list[str]) -> None:
        delimited = "__questpdf__".join(value)
        questpdf.lib.questpdf__settings__font_discovery_paths(delimited.encode('utf-8'))


def to_points(value: float, unit: Unit) -> float:
    return value

class PageSize:
    """
    Defines the physical dimensions (width and height) of a page.
    
    Commonly used page sizes are available in the PageSizes class.
    Change page orientation with the portrait() and landscape() methods.
    """
    
    width: float
    height: float

    def __init__(self, width: float, height: float, unit: Unit = Unit.point) -> None:
        """Create a PageSize with dimensions in the specified unit."""
        if width < 0:
            raise ValueError("Page width must be greater than 0.")

        if height < 0:
            raise ValueError("Page height must be greater than 0.")

        self.width = to_points(width, unit)
        self.height = to_points(height, unit)
    
    def portrait(self) -> 'PageSize':
        """Sets page size to portrait orientation, making width smaller than height."""
        return PageSize(
            width=min(self.width, self.height),
            height=max(self.width, self.height)
        )
    
    def landscape(self) -> 'PageSize':
        """Sets page size to landscape orientation, making width bigger than height."""
        return PageSize(
            width=max(self.width, self.height),
            height=min(self.width, self.height)
        )


class PageSizes:
    """
    Contains predefined, common and standard page sizes,
    such as A4 with dimensions of 595 points in width and 842 points in height.
    """
    
    POINTS_PER_INCH: int = 72
    
    # ISO A Series
    A0 = PageSize(2384, 3370)
    A1 = PageSize(1684, 2384)
    A2 = PageSize(1191, 1684)
    A3 = PageSize(842, 1191)
    A4 = PageSize(595, 842)
    A5 = PageSize(420, 595)
    A6 = PageSize(298, 420)
    A7 = PageSize(210, 298)
    A8 = PageSize(147, 210)
    A9 = PageSize(105, 147)
    A10 = PageSize(74, 105)

    # ISO B Series
    B0 = PageSize(2835, 4008)
    B1 = PageSize(2004, 2835)
    B2 = PageSize(1417, 2004)
    B3 = PageSize(1001, 1417)
    B4 = PageSize(709, 1001)
    B5 = PageSize(499, 709)
    B6 = PageSize(354, 499)
    B7 = PageSize(249, 354)
    B8 = PageSize(176, 249)
    B9 = PageSize(125, 176)
    B10 = PageSize(88, 125)

    # ISO C Series (Envelopes)
    C0 = PageSize(2599, 3677)
    C1 = PageSize(1837, 2599)
    C2 = PageSize(1298, 1837)
    C3 = PageSize(918, 1298)
    C4 = PageSize(649, 918)
    C5 = PageSize(459, 649)
    C6 = PageSize(323, 459)
    C7 = PageSize(230, 323)
    C8 = PageSize(162, 230)
    C9 = PageSize(113, 162)
    C10 = PageSize(79, 113)

    # Envelopes
    ENV_10 = PageSize(297, 684)
    ENV_C4 = PageSize(649, 918)
    ENV_DL = PageSize(312, 624)

    # North American
    POSTCARD = PageSize(284, 419)
    EXECUTIVE = PageSize(522, 756)
    LETTER = PageSize(612, 792)
    LEGAL = PageSize(612, 1008)
    LEDGER = PageSize(792, 1224)
    TABLOID = PageSize(1224, 792)

    # Architectural
    ARCH_A = PageSize(648, 864)
    ARCH_B = PageSize(864, 1296)
    ARCH_C = PageSize(1296, 1728)
    ARCH_D = PageSize(1728, 2592)
    ARCH_E = PageSize(2592, 3456)
    ARCH_E1 = PageSize(2160, 3024)
    ARCH_E2 = PageSize(1872, 2736)
    ARCH_E3 = PageSize(1944, 2808)


QUESTPDF_CDEF += r"""
    

    
    const char* questpdf__font_features__character_variant__3a2f9f96(int32_t value);
    
    const char* questpdf__font_features__stylistic_set__19e9e157(int32_t value);
    


"""



class FontFeatures:
    def __init__(self, target_pointer):
        self.target_pointer = target_pointer
        self._callbacks = []  # prevent GC of callbacks during native calls


    
    @staticmethod
    def character_variant(value: int) -> 'str':


        result = questpdf.lib.questpdf__font_features__character_variant__3a2f9f96(value)
        return decode_text_as_utf_8(result)

    
    @staticmethod
    def stylistic_set(value: int) -> 'str':


        result = questpdf.lib.questpdf__font_features__stylistic_set__19e9e157(value)
        return decode_text_as_utf_8(result)






QUESTPDF_CDEF += r"""
    

    
    const char* questpdf__placeholders__lorem_ipsum__39e2dfef();
    
    const char* questpdf__placeholders__label__06cbd381();
    
    const char* questpdf__placeholders__sentence__d64f6e68();
    
    const char* questpdf__placeholders__question__21de659f();
    
    const char* questpdf__placeholders__paragraph__56235510();
    
    const char* questpdf__placeholders__paragraphs__addb8834();
    
    const char* questpdf__placeholders__email__e6405590();
    
    const char* questpdf__placeholders__name__e536835d();
    
    const char* questpdf__placeholders__phone_number__25314f3f();
    
    const char* questpdf__placeholders__webpage_url__6e903669();
    
    const char* questpdf__placeholders__price__0ae34c02();
    
    const char* questpdf__placeholders__time__af3e33a8();
    
    const char* questpdf__placeholders__short_date__f6b7ade4();
    
    const char* questpdf__placeholders__long_date__736f3796();
    
    const char* questpdf__placeholders__date_time__b29d44f5();
    
    const char* questpdf__placeholders__integer__e8bfa000();
    
    const char* questpdf__placeholders__decimal__4569a5a1();
    
    const char* questpdf__placeholders__percent__3fa245e1();
    
    uint32_t questpdf__placeholders__background_color__5395a374();
    
    uint32_t questpdf__placeholders__color__40d13de0();
    
    void* questpdf__placeholders__image__a8827df6(int32_t width, int32_t height);
    
    void* questpdf__placeholders__image__2891dd9e(void* size);
    


"""



class Placeholders:
    def __init__(self, target_pointer):
        self.target_pointer = target_pointer
        self._callbacks = []  # prevent GC of callbacks during native calls


    
    @staticmethod
    def lorem_ipsum() -> 'str':


        result = questpdf.lib.questpdf__placeholders__lorem_ipsum__39e2dfef()
        return decode_text_as_utf_8(result)

    
    @staticmethod
    def label() -> 'str':


        result = questpdf.lib.questpdf__placeholders__label__06cbd381()
        return decode_text_as_utf_8(result)

    
    @staticmethod
    def sentence() -> 'str':


        result = questpdf.lib.questpdf__placeholders__sentence__d64f6e68()
        return decode_text_as_utf_8(result)

    
    @staticmethod
    def question() -> 'str':


        result = questpdf.lib.questpdf__placeholders__question__21de659f()
        return decode_text_as_utf_8(result)

    
    @staticmethod
    def paragraph() -> 'str':


        result = questpdf.lib.questpdf__placeholders__paragraph__56235510()
        return decode_text_as_utf_8(result)

    
    @staticmethod
    def paragraphs() -> 'str':


        result = questpdf.lib.questpdf__placeholders__paragraphs__addb8834()
        return decode_text_as_utf_8(result)

    
    @staticmethod
    def email() -> 'str':


        result = questpdf.lib.questpdf__placeholders__email__e6405590()
        return decode_text_as_utf_8(result)

    
    @staticmethod
    def name() -> 'str':


        result = questpdf.lib.questpdf__placeholders__name__e536835d()
        return decode_text_as_utf_8(result)

    
    @staticmethod
    def phone_number() -> 'str':


        result = questpdf.lib.questpdf__placeholders__phone_number__25314f3f()
        return decode_text_as_utf_8(result)

    
    @staticmethod
    def webpage_url() -> 'str':


        result = questpdf.lib.questpdf__placeholders__webpage_url__6e903669()
        return decode_text_as_utf_8(result)

    
    @staticmethod
    def price() -> 'str':


        result = questpdf.lib.questpdf__placeholders__price__0ae34c02()
        return decode_text_as_utf_8(result)

    
    @staticmethod
    def time() -> 'str':


        result = questpdf.lib.questpdf__placeholders__time__af3e33a8()
        return decode_text_as_utf_8(result)

    
    @staticmethod
    def short_date() -> 'str':


        result = questpdf.lib.questpdf__placeholders__short_date__f6b7ade4()
        return decode_text_as_utf_8(result)

    
    @staticmethod
    def long_date() -> 'str':


        result = questpdf.lib.questpdf__placeholders__long_date__736f3796()
        return decode_text_as_utf_8(result)

    
    @staticmethod
    def date_time() -> 'str':


        result = questpdf.lib.questpdf__placeholders__date_time__b29d44f5()
        return decode_text_as_utf_8(result)

    
    @staticmethod
    def integer() -> 'str':


        result = questpdf.lib.questpdf__placeholders__integer__e8bfa000()
        return decode_text_as_utf_8(result)

    
    @staticmethod
    def decimal() -> 'str':


        result = questpdf.lib.questpdf__placeholders__decimal__4569a5a1()
        return decode_text_as_utf_8(result)

    
    @staticmethod
    def percent() -> 'str':


        result = questpdf.lib.questpdf__placeholders__percent__3fa245e1()
        return decode_text_as_utf_8(result)

    
    @staticmethod
    def background_color() -> 'Color':


        result = questpdf.lib.questpdf__placeholders__background_color__5395a374()
        return Color(result)

    
    @staticmethod
    def color() -> 'Color':


        result = questpdf.lib.questpdf__placeholders__color__40d13de0()
        return Color(result)

    
    @staticmethod
    def _image_int_int(width: int, height: int) -> 'bytes':


        result = questpdf.lib.questpdf__placeholders__image__a8827df6(width, height)
        return questpdf_ffi.string(result)

    
    @staticmethod
    def _image_image_size(size: ImageSize) -> 'bytes':


        result = questpdf.lib.questpdf__placeholders__image__2891dd9e(size)
        return questpdf_ffi.string(result)


    @overload
    @staticmethod
    def image(width: int, height: int) -> 'bytes': ...

    @overload
    @staticmethod
    def image(size: ImageSize) -> 'bytes': ...

    @staticmethod
    def image(*args) -> 'bytes':
        if isinstance(args[0], int):
            return Placeholders._image_int_int(args[0], args[1])

        elif isinstance(args[0], ImageSize):
            return Placeholders._image_image_size(args[0])

        else:
            raise TypeError(f"Expected (int, int) or ImageSize, got {type(args[0])}")



QUESTPDF_CDEF += r"""
    

    


"""



class FontManager:
    def __init__(self, target_pointer):
        self.target_pointer = target_pointer
        self._callbacks = []  # prevent GC of callbacks during native calls







QUESTPDF_CDEF += r"""
    

    
    void* questpdf__image__from_file__d83c4447(const char* filePath);
    


"""



class Image:
    def __init__(self, target_pointer):
        self.target_pointer = target_pointer
        self._callbacks = []  # prevent GC of callbacks during native calls


    
    @staticmethod
    def from_file(file_path: str) -> 'Image':


        result = questpdf.lib.questpdf__image__from_file__d83c4447(questpdf_ffi.new("char[]", file_path.encode("utf-8")))
        return Image(result)






QUESTPDF_CDEF += r"""
    

    
    void* questpdf__svg_image__from_file__79e2c64d(const char* filePath);
    
    void* questpdf__svg_image__from_text__82b4cf0f(const char* svg);
    


"""



class SvgImage:
    def __init__(self, target_pointer):
        self.target_pointer = target_pointer
        self._callbacks = []  # prevent GC of callbacks during native calls


    
    @staticmethod
    def from_file(file_path: str) -> 'SvgImage':


        result = questpdf.lib.questpdf__svg_image__from_file__79e2c64d(questpdf_ffi.new("char[]", file_path.encode("utf-8")))
        return SvgImage(result)

    
    @staticmethod
    def from_text(svg: str) -> 'SvgImage':


        result = questpdf.lib.questpdf__svg_image__from_text__82b4cf0f(questpdf_ffi.new("char[]", svg.encode("utf-8")))
        return SvgImage(result)






QUESTPDF_CDEF += r"""
    

    
    void* questpdf__line_descriptor__line_color__a86ca4e3(void* target, uint32_t color);
    

    void* questpdf__line_descriptor__line_dash_pattern(void* target, float* values, int valuesLength, int unit);
    void* questpdf__line_descriptor__line_gradient(void* target, uint32_t* colors, int colorsLength);

"""



class LineDescriptor:
    def __init__(self, target_pointer):
        self.target_pointer = target_pointer
        self._callbacks = []  # prevent GC of callbacks during native calls


    
    
    def line_color(self, color: Color) -> 'LineDescriptor':


        result = questpdf.lib.questpdf__line_descriptor__line_color__a86ca4e3(self.target_pointer, color.hex)
        return LineDescriptor(result)


    def line_dash_pattern(self, pattern: list[float], unit: Unit = Unit.point) -> 'LineDescriptor':
        pattern_ffi = questpdf_ffi.new("float[]", pattern)
        result = questpdf.lib.questpdf__line_descriptor__line_dash_pattern(self.target_pointer, pattern_ffi, len(pattern_ffi), unit.value)
        return LineDescriptor(result)
    
    def line_gradient(self, colors: list[Color]) -> 'LineDescriptor':
        colors_hex = [color.hex for color in colors]
        colors_ffi = questpdf_ffi.new("uint32_t[]", colors_hex)
        result = questpdf.lib.questpdf__line_descriptor__line_gradient(self.target_pointer, colors_ffi, len(colors_ffi))
        return LineDescriptor(result)



QUESTPDF_CDEF += r"""
    

    
    void questpdf__column_descriptor__spacing__e47553e3(void* target, float value, int32_t unit);
    
    void* questpdf__column_descriptor__item__2cf2ad89(void* target);
    


"""



class ColumnDescriptor:
    def __init__(self, target_pointer):
        self.target_pointer = target_pointer
        self._callbacks = []  # prevent GC of callbacks during native calls


    
    
    def spacing(self, value: float, unit: Unit = Unit.point) -> None:


        result = questpdf.lib.questpdf__column_descriptor__spacing__e47553e3(self.target_pointer, value, unit.value)
        

    
    
    def item(self) -> 'Container':


        result = questpdf.lib.questpdf__column_descriptor__item__2cf2ad89(self.target_pointer)
        return Container(result)






QUESTPDF_CDEF += r"""
    
    typedef void (*container_callback_bf5ce29e)(void*);
    
    typedef void (*container_callback_391a971a)(void*);
    
    typedef void (*container_callback_4c35dd57)(void*);
    

    
    void* questpdf__decoration_descriptor__before__1bfecdf8(void* target);
    
    void questpdf__decoration_descriptor__before__bf5ce29e(void* target, container_callback_bf5ce29e handler);
    
    void* questpdf__decoration_descriptor__content__9ec35667(void* target);
    
    void questpdf__decoration_descriptor__content__391a971a(void* target, container_callback_391a971a handler);
    
    void* questpdf__decoration_descriptor__after__4cf66f67(void* target);
    
    void questpdf__decoration_descriptor__after__4c35dd57(void* target, container_callback_4c35dd57 handler);
    


"""



class DecorationDescriptor:
    def __init__(self, target_pointer):
        self.target_pointer = target_pointer
        self._callbacks = []  # prevent GC of callbacks during native calls


    
    
    def _before_no_args(self) -> 'Container':


        result = questpdf.lib.questpdf__decoration_descriptor__before__1bfecdf8(self.target_pointer)
        return Container(result)

    
    
    def _before_action(self, handler: Callable[['Container'], Any]) -> None:

        @questpdf_ffi.callback("void(void*)")
        def _internal_handler_handler(handler_pointer):
            handler_object = Container(handler_pointer)
            handler(handler_object)

        self._callbacks.append(_internal_handler_handler)


        result = questpdf.lib.questpdf__decoration_descriptor__before__bf5ce29e(self.target_pointer, _internal_handler_handler)
        

    
    
    def _content_no_args(self) -> 'Container':


        result = questpdf.lib.questpdf__decoration_descriptor__content__9ec35667(self.target_pointer)
        return Container(result)

    
    
    def _content_action(self, handler: Callable[['Container'], Any]) -> None:

        @questpdf_ffi.callback("void(void*)")
        def _internal_handler_handler(handler_pointer):
            handler_object = Container(handler_pointer)
            handler(handler_object)

        self._callbacks.append(_internal_handler_handler)


        result = questpdf.lib.questpdf__decoration_descriptor__content__391a971a(self.target_pointer, _internal_handler_handler)
        

    
    
    def _after_no_args(self) -> 'Container':


        result = questpdf.lib.questpdf__decoration_descriptor__after__4cf66f67(self.target_pointer)
        return Container(result)

    
    
    def _after_action(self, handler: Callable[['Container'], Any]) -> None:

        @questpdf_ffi.callback("void(void*)")
        def _internal_handler_handler(handler_pointer):
            handler_object = Container(handler_pointer)
            handler(handler_object)

        self._callbacks.append(_internal_handler_handler)


        result = questpdf.lib.questpdf__decoration_descriptor__after__4c35dd57(self.target_pointer, _internal_handler_handler)
        






QUESTPDF_CDEF += r"""
    

    
    void questpdf__inlined_descriptor__spacing__e466eaa7(void* target, float value, int32_t unit);
    
    void questpdf__inlined_descriptor__vertical_spacing__44456280(void* target, float value, int32_t unit);
    
    void questpdf__inlined_descriptor__horizontal_spacing__a035fbb4(void* target, float value, int32_t unit);
    
    void questpdf__inlined_descriptor__baseline_top__96b48f7f(void* target);
    
    void questpdf__inlined_descriptor__baseline_middle__2ee97366(void* target);
    
    void questpdf__inlined_descriptor__baseline_bottom__1878876e(void* target);
    
    void questpdf__inlined_descriptor__align_left__0c3a1762(void* target);
    
    void questpdf__inlined_descriptor__align_center__d09c92f2(void* target);
    
    void questpdf__inlined_descriptor__align_right__99b3ac01(void* target);
    
    void questpdf__inlined_descriptor__align_justify__3f036912(void* target);
    
    void questpdf__inlined_descriptor__align_space_around__cfaed88d(void* target);
    
    void* questpdf__inlined_descriptor__item__3a4e6d7b(void* target);
    


"""



class InlinedDescriptor:
    def __init__(self, target_pointer):
        self.target_pointer = target_pointer
        self._callbacks = []  # prevent GC of callbacks during native calls


    
    
    def spacing(self, value: float, unit: Unit = Unit.point) -> None:


        result = questpdf.lib.questpdf__inlined_descriptor__spacing__e466eaa7(self.target_pointer, value, unit.value)
        

    
    
    def vertical_spacing(self, value: float, unit: Unit = Unit.point) -> None:


        result = questpdf.lib.questpdf__inlined_descriptor__vertical_spacing__44456280(self.target_pointer, value, unit.value)
        

    
    
    def horizontal_spacing(self, value: float, unit: Unit = Unit.point) -> None:


        result = questpdf.lib.questpdf__inlined_descriptor__horizontal_spacing__a035fbb4(self.target_pointer, value, unit.value)
        

    
    
    def baseline_top(self) -> None:


        result = questpdf.lib.questpdf__inlined_descriptor__baseline_top__96b48f7f(self.target_pointer)
        

    
    
    def baseline_middle(self) -> None:


        result = questpdf.lib.questpdf__inlined_descriptor__baseline_middle__2ee97366(self.target_pointer)
        

    
    
    def baseline_bottom(self) -> None:


        result = questpdf.lib.questpdf__inlined_descriptor__baseline_bottom__1878876e(self.target_pointer)
        

    
    
    def align_left(self) -> None:


        result = questpdf.lib.questpdf__inlined_descriptor__align_left__0c3a1762(self.target_pointer)
        

    
    
    def align_center(self) -> None:


        result = questpdf.lib.questpdf__inlined_descriptor__align_center__d09c92f2(self.target_pointer)
        

    
    
    def align_right(self) -> None:


        result = questpdf.lib.questpdf__inlined_descriptor__align_right__99b3ac01(self.target_pointer)
        

    
    
    def align_justify(self) -> None:


        result = questpdf.lib.questpdf__inlined_descriptor__align_justify__3f036912(self.target_pointer)
        

    
    
    def align_space_around(self) -> None:


        result = questpdf.lib.questpdf__inlined_descriptor__align_space_around__cfaed88d(self.target_pointer)
        

    
    
    def item(self) -> 'Container':


        result = questpdf.lib.questpdf__inlined_descriptor__item__3a4e6d7b(self.target_pointer)
        return Container(result)






QUESTPDF_CDEF += r"""
    

    
    void* questpdf__layers_descriptor__layer__f8c1dd4f(void* target);
    
    void* questpdf__layers_descriptor__primary_layer__c2eb4a19(void* target);
    


"""



class LayersDescriptor:
    def __init__(self, target_pointer):
        self.target_pointer = target_pointer
        self._callbacks = []  # prevent GC of callbacks during native calls


    
    
    def layer(self) -> 'Container':


        result = questpdf.lib.questpdf__layers_descriptor__layer__f8c1dd4f(self.target_pointer)
        return Container(result)

    
    
    def primary_layer(self) -> 'Container':


        result = questpdf.lib.questpdf__layers_descriptor__primary_layer__c2eb4a19(self.target_pointer)
        return Container(result)






QUESTPDF_CDEF += r"""
    

    
    void questpdf__row_descriptor__spacing__09cc7a62(void* target, float spacing, int32_t unit);
    
    void* questpdf__row_descriptor__relative_item__f4570b47(void* target, float size);
    
    void* questpdf__row_descriptor__constant_item__4f927836(void* target, float size, int32_t unit);
    
    void* questpdf__row_descriptor__auto_item__fc084be8(void* target);
    


"""



class RowDescriptor:
    def __init__(self, target_pointer):
        self.target_pointer = target_pointer
        self._callbacks = []  # prevent GC of callbacks during native calls


    
    
    def spacing(self, spacing: float, unit: Unit = Unit.point) -> None:


        result = questpdf.lib.questpdf__row_descriptor__spacing__09cc7a62(self.target_pointer, spacing, unit.value)
        

    
    
    def relative_item(self, size: float = 1) -> 'Container':


        result = questpdf.lib.questpdf__row_descriptor__relative_item__f4570b47(self.target_pointer, size)
        return Container(result)

    
    
    def constant_item(self, size: float, unit: Unit = Unit.point) -> 'Container':


        result = questpdf.lib.questpdf__row_descriptor__constant_item__4f927836(self.target_pointer, size, unit.value)
        return Container(result)

    
    
    def auto_item(self) -> 'Container':


        result = questpdf.lib.questpdf__row_descriptor__auto_item__fc084be8(self.target_pointer)
        return Container(result)






QUESTPDF_CDEF += r"""
    

    
    void questpdf__grid_descriptor__spacing__2a69d201(void* target, float value, int32_t unit);
    
    void questpdf__grid_descriptor__vertical_spacing__593ca4c3(void* target, float value, int32_t unit);
    
    void questpdf__grid_descriptor__horizontal_spacing__a9d6ceae(void* target, float value, int32_t unit);
    
    void questpdf__grid_descriptor__columns__160f5f35(void* target, int32_t value);
    
    void questpdf__grid_descriptor__align_left__fc5e4cb9(void* target);
    
    void questpdf__grid_descriptor__align_center__3d81b2fe(void* target);
    
    void questpdf__grid_descriptor__align_right__e9aa71bc(void* target);
    
    void* questpdf__grid_descriptor__item__3e7cf6ba(void* target, int32_t columns);
    


"""



class GridDescriptor:
    def __init__(self, target_pointer):
        self.target_pointer = target_pointer
        self._callbacks = []  # prevent GC of callbacks during native calls


    
    
    def spacing(self, value: float, unit: Unit = Unit.point) -> None:


        result = questpdf.lib.questpdf__grid_descriptor__spacing__2a69d201(self.target_pointer, value, unit.value)
        

    
    
    def vertical_spacing(self, value: float, unit: Unit = Unit.point) -> None:


        result = questpdf.lib.questpdf__grid_descriptor__vertical_spacing__593ca4c3(self.target_pointer, value, unit.value)
        

    
    
    def horizontal_spacing(self, value: float, unit: Unit = Unit.point) -> None:


        result = questpdf.lib.questpdf__grid_descriptor__horizontal_spacing__a9d6ceae(self.target_pointer, value, unit.value)
        

    
    
    def columns(self, value: int = 12) -> None:


        result = questpdf.lib.questpdf__grid_descriptor__columns__160f5f35(self.target_pointer, value)
        

    
    
    def align_left(self) -> None:


        result = questpdf.lib.questpdf__grid_descriptor__align_left__fc5e4cb9(self.target_pointer)
        

    
    
    def align_center(self) -> None:


        result = questpdf.lib.questpdf__grid_descriptor__align_center__3d81b2fe(self.target_pointer)
        

    
    
    def align_right(self) -> None:


        result = questpdf.lib.questpdf__grid_descriptor__align_right__e9aa71bc(self.target_pointer)
        

    
    
    def item(self, columns: int = 1) -> 'Container':


        result = questpdf.lib.questpdf__grid_descriptor__item__3e7cf6ba(self.target_pointer, columns)
        return Container(result)






QUESTPDF_CDEF += r"""
    

    
    void questpdf__multi_column_descriptor__spacing__b96a0ed7(void* target, float value, int32_t unit);
    
    void questpdf__multi_column_descriptor__columns__f9027e4e(void* target, int32_t value);
    
    void questpdf__multi_column_descriptor__balance_height__a0509325(void* target, uint8_t enable);
    
    void* questpdf__multi_column_descriptor__content__68196264(void* target);
    
    void* questpdf__multi_column_descriptor__spacer__9d6eea5d(void* target);
    


"""



class MultiColumnDescriptor:
    def __init__(self, target_pointer):
        self.target_pointer = target_pointer
        self._callbacks = []  # prevent GC of callbacks during native calls


    
    
    def spacing(self, value: float, unit: Unit = Unit.point) -> None:


        result = questpdf.lib.questpdf__multi_column_descriptor__spacing__b96a0ed7(self.target_pointer, value, unit.value)
        

    
    
    def columns(self, value: int = 2) -> None:


        result = questpdf.lib.questpdf__multi_column_descriptor__columns__f9027e4e(self.target_pointer, value)
        

    
    
    def balance_height(self, enable: bool = True) -> None:


        result = questpdf.lib.questpdf__multi_column_descriptor__balance_height__a0509325(self.target_pointer, enable)
        

    
    
    def content(self) -> 'Container':


        result = questpdf.lib.questpdf__multi_column_descriptor__content__68196264(self.target_pointer)
        return Container(result)

    
    
    def spacer(self) -> 'Container':


        result = questpdf.lib.questpdf__multi_column_descriptor__spacer__9d6eea5d(self.target_pointer)
        return Container(result)






QUESTPDF_CDEF += r"""
    

    
    void* questpdf__table_cell_descriptor__cell__1061edf9(void* target);
    


"""



class TableCellDescriptor:
    def __init__(self, target_pointer):
        self.target_pointer = target_pointer
        self._callbacks = []  # prevent GC of callbacks during native calls


    
    
    def cell(self) -> 'TableCellContainer':


        result = questpdf.lib.questpdf__table_cell_descriptor__cell__1061edf9(self.target_pointer)
        return TableCellContainer(result)






QUESTPDF_CDEF += r"""
    

    
    void questpdf__table_columns_definition_descriptor__constant_column__e71e4979(void* target, float width, int32_t unit);
    
    void questpdf__table_columns_definition_descriptor__relative_column__940a67b1(void* target, float width);
    


"""



class TableColumnsDefinitionDescriptor:
    def __init__(self, target_pointer):
        self.target_pointer = target_pointer
        self._callbacks = []  # prevent GC of callbacks during native calls


    
    
    def constant_column(self, width: float, unit: Unit = Unit.point) -> None:


        result = questpdf.lib.questpdf__table_columns_definition_descriptor__constant_column__e71e4979(self.target_pointer, width, unit.value)
        

    
    
    def relative_column(self, width: float = 1) -> None:


        result = questpdf.lib.questpdf__table_columns_definition_descriptor__relative_column__940a67b1(self.target_pointer, width)
        






QUESTPDF_CDEF += r"""
    
    typedef void (*table_columns_definition_descriptor_callback_1b198f41)(void*);
    
    typedef void (*table_cell_descriptor_callback_227448b3)(void*);
    
    typedef void (*table_cell_descriptor_callback_a74a23a5)(void*);
    

    
    void questpdf__table_descriptor__columns_definition__1b198f41(void* target, table_columns_definition_descriptor_callback_1b198f41 handler);
    
    void questpdf__table_descriptor__extend_last_cells_to_table_bottom__22a2235b(void* target);
    
    void questpdf__table_descriptor__header__227448b3(void* target, table_cell_descriptor_callback_227448b3 handler);
    
    void questpdf__table_descriptor__footer__a74a23a5(void* target, table_cell_descriptor_callback_a74a23a5 handler);
    
    void* questpdf__table_descriptor__cell__1f40892e(void* target);
    


"""



class TableDescriptor:
    def __init__(self, target_pointer):
        self.target_pointer = target_pointer
        self._callbacks = []  # prevent GC of callbacks during native calls


    
    
    def columns_definition(self, handler: Callable[['TableColumnsDefinitionDescriptor'], Any]) -> None:

        @questpdf_ffi.callback("void(void*)")
        def _internal_handler_handler(handler_pointer):
            handler_object = TableColumnsDefinitionDescriptor(handler_pointer)
            handler(handler_object)

        self._callbacks.append(_internal_handler_handler)


        result = questpdf.lib.questpdf__table_descriptor__columns_definition__1b198f41(self.target_pointer, _internal_handler_handler)
        

    
    
    def extend_last_cells_to_table_bottom(self) -> None:


        result = questpdf.lib.questpdf__table_descriptor__extend_last_cells_to_table_bottom__22a2235b(self.target_pointer)
        

    
    
    def header(self, handler: Callable[['TableCellDescriptor'], Any]) -> None:

        @questpdf_ffi.callback("void(void*)")
        def _internal_handler_handler(handler_pointer):
            handler_object = TableCellDescriptor(handler_pointer)
            handler(handler_object)

        self._callbacks.append(_internal_handler_handler)


        result = questpdf.lib.questpdf__table_descriptor__header__227448b3(self.target_pointer, _internal_handler_handler)
        

    
    
    def footer(self, handler: Callable[['TableCellDescriptor'], Any]) -> None:

        @questpdf_ffi.callback("void(void*)")
        def _internal_handler_handler(handler_pointer):
            handler_object = TableCellDescriptor(handler_pointer)
            handler(handler_object)

        self._callbacks.append(_internal_handler_handler)


        result = questpdf.lib.questpdf__table_descriptor__footer__a74a23a5(self.target_pointer, _internal_handler_handler)
        

    
    
    def cell(self) -> 'TableCellContainer':


        result = questpdf.lib.questpdf__table_descriptor__cell__1f40892e(self.target_pointer)
        return TableCellContainer(result)






QUESTPDF_CDEF += r"""
    
    typedef void (*container_callback_ff63896d)(void*);
    

    
    void questpdf__text_descriptor__align_left__4a573634(void* target);
    
    void questpdf__text_descriptor__align_center__def2b616(void* target);
    
    void questpdf__text_descriptor__align_right__de6eaa17(void* target);
    
    void questpdf__text_descriptor__justify__1501b0fa(void* target);
    
    void questpdf__text_descriptor__align_start__947ba696(void* target);
    
    void questpdf__text_descriptor__align_end__5aefafc5(void* target);
    
    void questpdf__text_descriptor__clamp_lines__f1b02b03(void* target, int32_t maxLines, const char* ellipsis);
    
    void questpdf__text_descriptor__paragraph_spacing__c3629bd6(void* target, float value, int32_t unit);
    
    void questpdf__text_descriptor__paragraph_first_line_indentation__414498e7(void* target, float value, int32_t unit);
    
    void* questpdf__text_descriptor__span__41a383c0(void* target, const char* text);
    
    void* questpdf__text_descriptor__line__17db2520(void* target, const char* text);
    
    void* questpdf__text_descriptor__empty_line__70ae8fc0(void* target);
    
    void* questpdf__text_descriptor__current_page_number__2097e179(void* target);
    
    void* questpdf__text_descriptor__total_pages__604d3e19(void* target);
    
    void* questpdf__text_descriptor__begin_page_number_of_section__340accfc(void* target, const char* sectionName);
    
    void* questpdf__text_descriptor__end_page_number_of_section__deee569a(void* target, const char* sectionName);
    
    void* questpdf__text_descriptor__page_number_within_section__51768233(void* target, const char* sectionName);
    
    void* questpdf__text_descriptor__total_pages_within_section__250c06e5(void* target, const char* sectionName);
    
    void* questpdf__text_descriptor__section_link__c9b32c1a(void* target, const char* text, const char* sectionName);
    
    void* questpdf__text_descriptor__hyperlink__f38a28c7(void* target, const char* text, const char* url);
    
    void* questpdf__text_descriptor__element__862752ab(void* target, int32_t alignment);
    
    void questpdf__text_descriptor__element__ff63896d(void* target, container_callback_ff63896d handler, int32_t alignment);
    


"""



class TextDescriptor:
    def __init__(self, target_pointer):
        self.target_pointer = target_pointer
        self._callbacks = []  # prevent GC of callbacks during native calls


    
    
    def align_left(self) -> None:


        result = questpdf.lib.questpdf__text_descriptor__align_left__4a573634(self.target_pointer)
        

    
    
    def align_center(self) -> None:


        result = questpdf.lib.questpdf__text_descriptor__align_center__def2b616(self.target_pointer)
        

    
    
    def align_right(self) -> None:


        result = questpdf.lib.questpdf__text_descriptor__align_right__de6eaa17(self.target_pointer)
        

    
    
    def justify(self) -> None:


        result = questpdf.lib.questpdf__text_descriptor__justify__1501b0fa(self.target_pointer)
        

    
    
    def align_start(self) -> None:


        result = questpdf.lib.questpdf__text_descriptor__align_start__947ba696(self.target_pointer)
        

    
    
    def align_end(self) -> None:


        result = questpdf.lib.questpdf__text_descriptor__align_end__5aefafc5(self.target_pointer)
        

    
    
    def clamp_lines(self, max_lines: int, ellipsis: str = '…') -> None:


        result = questpdf.lib.questpdf__text_descriptor__clamp_lines__f1b02b03(self.target_pointer, max_lines, questpdf_ffi.new("char[]", ellipsis.encode("utf-8")))
        

    
    
    def paragraph_spacing(self, value: float, unit: Unit = Unit.point) -> None:


        result = questpdf.lib.questpdf__text_descriptor__paragraph_spacing__c3629bd6(self.target_pointer, value, unit.value)
        

    
    
    def paragraph_first_line_indentation(self, value: float, unit: Unit = Unit.point) -> None:


        result = questpdf.lib.questpdf__text_descriptor__paragraph_first_line_indentation__414498e7(self.target_pointer, value, unit.value)
        

    
    
    def span(self, text: str) -> 'TextSpanDescriptor':


        result = questpdf.lib.questpdf__text_descriptor__span__41a383c0(self.target_pointer, questpdf_ffi.new("char[]", text.encode("utf-8")))
        return TextSpanDescriptor(result)

    
    
    def line(self, text: str) -> 'TextSpanDescriptor':


        result = questpdf.lib.questpdf__text_descriptor__line__17db2520(self.target_pointer, questpdf_ffi.new("char[]", text.encode("utf-8")))
        return TextSpanDescriptor(result)

    
    
    def empty_line(self) -> 'TextSpanDescriptor':


        result = questpdf.lib.questpdf__text_descriptor__empty_line__70ae8fc0(self.target_pointer)
        return TextSpanDescriptor(result)

    
    
    def current_page_number(self) -> 'TextPageNumberDescriptor':


        result = questpdf.lib.questpdf__text_descriptor__current_page_number__2097e179(self.target_pointer)
        return TextPageNumberDescriptor(result)

    
    
    def total_pages(self) -> 'TextPageNumberDescriptor':


        result = questpdf.lib.questpdf__text_descriptor__total_pages__604d3e19(self.target_pointer)
        return TextPageNumberDescriptor(result)

    
    
    def begin_page_number_of_section(self, section_name: str) -> 'TextPageNumberDescriptor':


        result = questpdf.lib.questpdf__text_descriptor__begin_page_number_of_section__340accfc(self.target_pointer, questpdf_ffi.new("char[]", section_name.encode("utf-8")))
        return TextPageNumberDescriptor(result)

    
    
    def end_page_number_of_section(self, section_name: str) -> 'TextPageNumberDescriptor':


        result = questpdf.lib.questpdf__text_descriptor__end_page_number_of_section__deee569a(self.target_pointer, questpdf_ffi.new("char[]", section_name.encode("utf-8")))
        return TextPageNumberDescriptor(result)

    
    
    def page_number_within_section(self, section_name: str) -> 'TextPageNumberDescriptor':


        result = questpdf.lib.questpdf__text_descriptor__page_number_within_section__51768233(self.target_pointer, questpdf_ffi.new("char[]", section_name.encode("utf-8")))
        return TextPageNumberDescriptor(result)

    
    
    def total_pages_within_section(self, section_name: str) -> 'TextPageNumberDescriptor':


        result = questpdf.lib.questpdf__text_descriptor__total_pages_within_section__250c06e5(self.target_pointer, questpdf_ffi.new("char[]", section_name.encode("utf-8")))
        return TextPageNumberDescriptor(result)

    
    
    def section_link(self, text: str, section_name: str) -> 'TextSpanDescriptor':


        result = questpdf.lib.questpdf__text_descriptor__section_link__c9b32c1a(self.target_pointer, questpdf_ffi.new("char[]", text.encode("utf-8")), questpdf_ffi.new("char[]", section_name.encode("utf-8")))
        return TextSpanDescriptor(result)

    
    
    def hyperlink(self, text: str, url: str) -> 'TextSpanDescriptor':


        result = questpdf.lib.questpdf__text_descriptor__hyperlink__f38a28c7(self.target_pointer, questpdf_ffi.new("char[]", text.encode("utf-8")), questpdf_ffi.new("char[]", url.encode("utf-8")))
        return TextSpanDescriptor(result)

    
    
    def _element_text_injected_element_alignment(self, alignment: TextInjectedElementAlignment = TextInjectedElementAlignment.above_baseline) -> 'Container':


        result = questpdf.lib.questpdf__text_descriptor__element__862752ab(self.target_pointer, alignment.value)
        return Container(result)

    
    
    def _element_action_text_injected_element_alignment(self, handler: Callable[['Container'], Any], alignment: TextInjectedElementAlignment = TextInjectedElementAlignment.above_baseline) -> None:

        @questpdf_ffi.callback("void(void*)")
        def _internal_handler_handler(handler_pointer):
            handler_object = Container(handler_pointer)
            handler(handler_object)

        self._callbacks.append(_internal_handler_handler)


        result = questpdf.lib.questpdf__text_descriptor__element__ff63896d(self.target_pointer, _internal_handler_handler, alignment.value)
        






QUESTPDF_CDEF += r"""
    

    
    void* questpdf__text_span_descriptor__font_color__a0d06e42(void* descriptor, uint32_t color);
    
    void* questpdf__text_span_descriptor__background_color__5461b453(void* descriptor, uint32_t color);
    
    void* questpdf__text_span_descriptor__font_size__c989487d(void* descriptor, float value);
    
    void* questpdf__text_span_descriptor__line_height__a1b4697a(void* descriptor, void* factor);
    
    void* questpdf__text_span_descriptor__letter_spacing__92f86a26(void* descriptor, float factor);
    
    void* questpdf__text_span_descriptor__word_spacing__1f794add(void* descriptor, float factor);
    
    void* questpdf__text_span_descriptor__italic__4f023aba(void* descriptor, uint8_t value);
    
    void* questpdf__text_span_descriptor__strikethrough__41841206(void* descriptor, uint8_t value);
    
    void* questpdf__text_span_descriptor__underline__2e1ae473(void* descriptor, uint8_t value);
    
    void* questpdf__text_span_descriptor__overline__add25860(void* descriptor, uint8_t value);
    
    void* questpdf__text_span_descriptor__decoration_color__5d18d151(void* descriptor, uint32_t color);
    
    void* questpdf__text_span_descriptor__decoration_thickness__c7c23c84(void* descriptor, float factor);
    
    void* questpdf__text_span_descriptor__decoration_solid__f64746d1(void* descriptor);
    
    void* questpdf__text_span_descriptor__decoration_double__41cf8a18(void* descriptor);
    
    void* questpdf__text_span_descriptor__decoration_wavy__1761acf2(void* descriptor);
    
    void* questpdf__text_span_descriptor__decoration_dotted__e940537a(void* descriptor);
    
    void* questpdf__text_span_descriptor__decoration_dashed__a85f7344(void* descriptor);
    
    void* questpdf__text_span_descriptor__thin__e9036638(void* descriptor);
    
    void* questpdf__text_span_descriptor__extra_light__33bbe020(void* descriptor);
    
    void* questpdf__text_span_descriptor__light__37ef1bc2(void* descriptor);
    
    void* questpdf__text_span_descriptor__normal_weight__18d360b3(void* descriptor);
    
    void* questpdf__text_span_descriptor__medium__5ef8b80e(void* descriptor);
    
    void* questpdf__text_span_descriptor__semi_bold__0b92f7b7(void* descriptor);
    
    void* questpdf__text_span_descriptor__bold__0dfa9061(void* descriptor);
    
    void* questpdf__text_span_descriptor__extra_bold__c4fbc0a6(void* descriptor);
    
    void* questpdf__text_span_descriptor__black__0cc8d698(void* descriptor);
    
    void* questpdf__text_span_descriptor__extra_black__c7698d85(void* descriptor);
    
    void* questpdf__text_span_descriptor__normal_position__5e5176cb(void* descriptor);
    
    void* questpdf__text_span_descriptor__subscript__db9bd4eb(void* descriptor);
    
    void* questpdf__text_span_descriptor__superscript__a9b46b1e(void* descriptor);
    
    void* questpdf__text_span_descriptor__direction_auto__fbed9e71(void* descriptor);
    
    void* questpdf__text_span_descriptor__direction_from_left_to_right__09e2e3bc(void* descriptor);
    
    void* questpdf__text_span_descriptor__direction_from_right_to_left__6cc5cb9e(void* descriptor);
    
    void* questpdf__text_span_descriptor__enable_font_feature__136a164d(void* descriptor, const char* featureName);
    
    void* questpdf__text_span_descriptor__disable_font_feature__5bd81de9(void* descriptor, const char* featureName);
    


"""



class TextSpanDescriptor:
    def __init__(self, target_pointer):
        self.target_pointer = target_pointer
        self._callbacks = []  # prevent GC of callbacks during native calls


    
    
    def font_color(self, color: Color) -> 'TextSpanDescriptor':


        result = questpdf.lib.questpdf__text_span_descriptor__font_color__a0d06e42(self.target_pointer, color.hex)
        return TextSpanDescriptor(result)

    
    
    def background_color(self, color: Color) -> 'TextSpanDescriptor':


        result = questpdf.lib.questpdf__text_span_descriptor__background_color__5461b453(self.target_pointer, color.hex)
        return TextSpanDescriptor(result)

    
    
    def font_size(self, value: float) -> 'TextSpanDescriptor':


        result = questpdf.lib.questpdf__text_span_descriptor__font_size__c989487d(self.target_pointer, value)
        return TextSpanDescriptor(result)

    
    
    def line_height(self, factor: Any) -> 'TextSpanDescriptor':


        result = questpdf.lib.questpdf__text_span_descriptor__line_height__a1b4697a(self.target_pointer, factor)
        return TextSpanDescriptor(result)

    
    
    def letter_spacing(self, factor: float = 0) -> 'TextSpanDescriptor':


        result = questpdf.lib.questpdf__text_span_descriptor__letter_spacing__92f86a26(self.target_pointer, factor)
        return TextSpanDescriptor(result)

    
    
    def word_spacing(self, factor: float = 0) -> 'TextSpanDescriptor':


        result = questpdf.lib.questpdf__text_span_descriptor__word_spacing__1f794add(self.target_pointer, factor)
        return TextSpanDescriptor(result)

    
    
    def italic(self, value: bool = True) -> 'TextSpanDescriptor':


        result = questpdf.lib.questpdf__text_span_descriptor__italic__4f023aba(self.target_pointer, value)
        return TextSpanDescriptor(result)

    
    
    def strikethrough(self, value: bool = True) -> 'TextSpanDescriptor':


        result = questpdf.lib.questpdf__text_span_descriptor__strikethrough__41841206(self.target_pointer, value)
        return TextSpanDescriptor(result)

    
    
    def underline(self, value: bool = True) -> 'TextSpanDescriptor':


        result = questpdf.lib.questpdf__text_span_descriptor__underline__2e1ae473(self.target_pointer, value)
        return TextSpanDescriptor(result)

    
    
    def overline(self, value: bool = True) -> 'TextSpanDescriptor':


        result = questpdf.lib.questpdf__text_span_descriptor__overline__add25860(self.target_pointer, value)
        return TextSpanDescriptor(result)

    
    
    def decoration_color(self, color: Color) -> 'TextSpanDescriptor':


        result = questpdf.lib.questpdf__text_span_descriptor__decoration_color__5d18d151(self.target_pointer, color.hex)
        return TextSpanDescriptor(result)

    
    
    def decoration_thickness(self, factor: float) -> 'TextSpanDescriptor':


        result = questpdf.lib.questpdf__text_span_descriptor__decoration_thickness__c7c23c84(self.target_pointer, factor)
        return TextSpanDescriptor(result)

    
    
    def decoration_solid(self) -> 'TextSpanDescriptor':


        result = questpdf.lib.questpdf__text_span_descriptor__decoration_solid__f64746d1(self.target_pointer)
        return TextSpanDescriptor(result)

    
    
    def decoration_double(self) -> 'TextSpanDescriptor':


        result = questpdf.lib.questpdf__text_span_descriptor__decoration_double__41cf8a18(self.target_pointer)
        return TextSpanDescriptor(result)

    
    
    def decoration_wavy(self) -> 'TextSpanDescriptor':


        result = questpdf.lib.questpdf__text_span_descriptor__decoration_wavy__1761acf2(self.target_pointer)
        return TextSpanDescriptor(result)

    
    
    def decoration_dotted(self) -> 'TextSpanDescriptor':


        result = questpdf.lib.questpdf__text_span_descriptor__decoration_dotted__e940537a(self.target_pointer)
        return TextSpanDescriptor(result)

    
    
    def decoration_dashed(self) -> 'TextSpanDescriptor':


        result = questpdf.lib.questpdf__text_span_descriptor__decoration_dashed__a85f7344(self.target_pointer)
        return TextSpanDescriptor(result)

    
    
    def thin(self) -> 'TextSpanDescriptor':


        result = questpdf.lib.questpdf__text_span_descriptor__thin__e9036638(self.target_pointer)
        return TextSpanDescriptor(result)

    
    
    def extra_light(self) -> 'TextSpanDescriptor':


        result = questpdf.lib.questpdf__text_span_descriptor__extra_light__33bbe020(self.target_pointer)
        return TextSpanDescriptor(result)

    
    
    def light(self) -> 'TextSpanDescriptor':


        result = questpdf.lib.questpdf__text_span_descriptor__light__37ef1bc2(self.target_pointer)
        return TextSpanDescriptor(result)

    
    
    def normal_weight(self) -> 'TextSpanDescriptor':


        result = questpdf.lib.questpdf__text_span_descriptor__normal_weight__18d360b3(self.target_pointer)
        return TextSpanDescriptor(result)

    
    
    def medium(self) -> 'TextSpanDescriptor':


        result = questpdf.lib.questpdf__text_span_descriptor__medium__5ef8b80e(self.target_pointer)
        return TextSpanDescriptor(result)

    
    
    def semi_bold(self) -> 'TextSpanDescriptor':


        result = questpdf.lib.questpdf__text_span_descriptor__semi_bold__0b92f7b7(self.target_pointer)
        return TextSpanDescriptor(result)

    
    
    def bold(self) -> 'TextSpanDescriptor':


        result = questpdf.lib.questpdf__text_span_descriptor__bold__0dfa9061(self.target_pointer)
        return TextSpanDescriptor(result)

    
    
    def extra_bold(self) -> 'TextSpanDescriptor':


        result = questpdf.lib.questpdf__text_span_descriptor__extra_bold__c4fbc0a6(self.target_pointer)
        return TextSpanDescriptor(result)

    
    
    def black(self) -> 'TextSpanDescriptor':


        result = questpdf.lib.questpdf__text_span_descriptor__black__0cc8d698(self.target_pointer)
        return TextSpanDescriptor(result)

    
    
    def extra_black(self) -> 'TextSpanDescriptor':


        result = questpdf.lib.questpdf__text_span_descriptor__extra_black__c7698d85(self.target_pointer)
        return TextSpanDescriptor(result)

    
    
    def normal_position(self) -> 'TextSpanDescriptor':


        result = questpdf.lib.questpdf__text_span_descriptor__normal_position__5e5176cb(self.target_pointer)
        return TextSpanDescriptor(result)

    
    
    def subscript(self) -> 'TextSpanDescriptor':


        result = questpdf.lib.questpdf__text_span_descriptor__subscript__db9bd4eb(self.target_pointer)
        return TextSpanDescriptor(result)

    
    
    def superscript(self) -> 'TextSpanDescriptor':


        result = questpdf.lib.questpdf__text_span_descriptor__superscript__a9b46b1e(self.target_pointer)
        return TextSpanDescriptor(result)

    
    
    def direction_auto(self) -> 'TextSpanDescriptor':


        result = questpdf.lib.questpdf__text_span_descriptor__direction_auto__fbed9e71(self.target_pointer)
        return TextSpanDescriptor(result)

    
    
    def direction_from_left_to_right(self) -> 'TextSpanDescriptor':


        result = questpdf.lib.questpdf__text_span_descriptor__direction_from_left_to_right__09e2e3bc(self.target_pointer)
        return TextSpanDescriptor(result)

    
    
    def direction_from_right_to_left(self) -> 'TextSpanDescriptor':


        result = questpdf.lib.questpdf__text_span_descriptor__direction_from_right_to_left__6cc5cb9e(self.target_pointer)
        return TextSpanDescriptor(result)

    
    
    def enable_font_feature(self, feature_name: str) -> 'TextSpanDescriptor':


        result = questpdf.lib.questpdf__text_span_descriptor__enable_font_feature__136a164d(self.target_pointer, questpdf_ffi.new("char[]", feature_name.encode("utf-8")))
        return TextSpanDescriptor(result)

    
    
    def disable_font_feature(self, feature_name: str) -> 'TextSpanDescriptor':


        result = questpdf.lib.questpdf__text_span_descriptor__disable_font_feature__5bd81de9(self.target_pointer, questpdf_ffi.new("char[]", feature_name.encode("utf-8")))
        return TextSpanDescriptor(result)






QUESTPDF_CDEF += r"""
    

    
    void* questpdf__text_page_number_descriptor__font_color__a0d06e42(void* descriptor, uint32_t color);
    
    void* questpdf__text_page_number_descriptor__background_color__5461b453(void* descriptor, uint32_t color);
    
    void* questpdf__text_page_number_descriptor__font_size__c989487d(void* descriptor, float value);
    
    void* questpdf__text_page_number_descriptor__line_height__a1b4697a(void* descriptor, void* factor);
    
    void* questpdf__text_page_number_descriptor__letter_spacing__92f86a26(void* descriptor, float factor);
    
    void* questpdf__text_page_number_descriptor__word_spacing__1f794add(void* descriptor, float factor);
    
    void* questpdf__text_page_number_descriptor__italic__4f023aba(void* descriptor, uint8_t value);
    
    void* questpdf__text_page_number_descriptor__strikethrough__41841206(void* descriptor, uint8_t value);
    
    void* questpdf__text_page_number_descriptor__underline__2e1ae473(void* descriptor, uint8_t value);
    
    void* questpdf__text_page_number_descriptor__overline__add25860(void* descriptor, uint8_t value);
    
    void* questpdf__text_page_number_descriptor__decoration_color__5d18d151(void* descriptor, uint32_t color);
    
    void* questpdf__text_page_number_descriptor__decoration_thickness__c7c23c84(void* descriptor, float factor);
    
    void* questpdf__text_page_number_descriptor__decoration_solid__f64746d1(void* descriptor);
    
    void* questpdf__text_page_number_descriptor__decoration_double__41cf8a18(void* descriptor);
    
    void* questpdf__text_page_number_descriptor__decoration_wavy__1761acf2(void* descriptor);
    
    void* questpdf__text_page_number_descriptor__decoration_dotted__e940537a(void* descriptor);
    
    void* questpdf__text_page_number_descriptor__decoration_dashed__a85f7344(void* descriptor);
    
    void* questpdf__text_page_number_descriptor__thin__e9036638(void* descriptor);
    
    void* questpdf__text_page_number_descriptor__extra_light__33bbe020(void* descriptor);
    
    void* questpdf__text_page_number_descriptor__light__37ef1bc2(void* descriptor);
    
    void* questpdf__text_page_number_descriptor__normal_weight__18d360b3(void* descriptor);
    
    void* questpdf__text_page_number_descriptor__medium__5ef8b80e(void* descriptor);
    
    void* questpdf__text_page_number_descriptor__semi_bold__0b92f7b7(void* descriptor);
    
    void* questpdf__text_page_number_descriptor__bold__0dfa9061(void* descriptor);
    
    void* questpdf__text_page_number_descriptor__extra_bold__c4fbc0a6(void* descriptor);
    
    void* questpdf__text_page_number_descriptor__black__0cc8d698(void* descriptor);
    
    void* questpdf__text_page_number_descriptor__extra_black__c7698d85(void* descriptor);
    
    void* questpdf__text_page_number_descriptor__normal_position__5e5176cb(void* descriptor);
    
    void* questpdf__text_page_number_descriptor__subscript__db9bd4eb(void* descriptor);
    
    void* questpdf__text_page_number_descriptor__superscript__a9b46b1e(void* descriptor);
    
    void* questpdf__text_page_number_descriptor__direction_auto__fbed9e71(void* descriptor);
    
    void* questpdf__text_page_number_descriptor__direction_from_left_to_right__09e2e3bc(void* descriptor);
    
    void* questpdf__text_page_number_descriptor__direction_from_right_to_left__6cc5cb9e(void* descriptor);
    
    void* questpdf__text_page_number_descriptor__enable_font_feature__136a164d(void* descriptor, const char* featureName);
    
    void* questpdf__text_page_number_descriptor__disable_font_feature__5bd81de9(void* descriptor, const char* featureName);
    


"""



class TextPageNumberDescriptor:
    def __init__(self, target_pointer):
        self.target_pointer = target_pointer
        self._callbacks = []  # prevent GC of callbacks during native calls


    
    
    def font_color(self, color: Color) -> 'TextPageNumberDescriptor':


        result = questpdf.lib.questpdf__text_page_number_descriptor__font_color__a0d06e42(self.target_pointer, color.hex)
        return TextPageNumberDescriptor(result)

    
    
    def background_color(self, color: Color) -> 'TextPageNumberDescriptor':


        result = questpdf.lib.questpdf__text_page_number_descriptor__background_color__5461b453(self.target_pointer, color.hex)
        return TextPageNumberDescriptor(result)

    
    
    def font_size(self, value: float) -> 'TextPageNumberDescriptor':


        result = questpdf.lib.questpdf__text_page_number_descriptor__font_size__c989487d(self.target_pointer, value)
        return TextPageNumberDescriptor(result)

    
    
    def line_height(self, factor: Any) -> 'TextPageNumberDescriptor':


        result = questpdf.lib.questpdf__text_page_number_descriptor__line_height__a1b4697a(self.target_pointer, factor)
        return TextPageNumberDescriptor(result)

    
    
    def letter_spacing(self, factor: float = 0) -> 'TextPageNumberDescriptor':


        result = questpdf.lib.questpdf__text_page_number_descriptor__letter_spacing__92f86a26(self.target_pointer, factor)
        return TextPageNumberDescriptor(result)

    
    
    def word_spacing(self, factor: float = 0) -> 'TextPageNumberDescriptor':


        result = questpdf.lib.questpdf__text_page_number_descriptor__word_spacing__1f794add(self.target_pointer, factor)
        return TextPageNumberDescriptor(result)

    
    
    def italic(self, value: bool = True) -> 'TextPageNumberDescriptor':


        result = questpdf.lib.questpdf__text_page_number_descriptor__italic__4f023aba(self.target_pointer, value)
        return TextPageNumberDescriptor(result)

    
    
    def strikethrough(self, value: bool = True) -> 'TextPageNumberDescriptor':


        result = questpdf.lib.questpdf__text_page_number_descriptor__strikethrough__41841206(self.target_pointer, value)
        return TextPageNumberDescriptor(result)

    
    
    def underline(self, value: bool = True) -> 'TextPageNumberDescriptor':


        result = questpdf.lib.questpdf__text_page_number_descriptor__underline__2e1ae473(self.target_pointer, value)
        return TextPageNumberDescriptor(result)

    
    
    def overline(self, value: bool = True) -> 'TextPageNumberDescriptor':


        result = questpdf.lib.questpdf__text_page_number_descriptor__overline__add25860(self.target_pointer, value)
        return TextPageNumberDescriptor(result)

    
    
    def decoration_color(self, color: Color) -> 'TextPageNumberDescriptor':


        result = questpdf.lib.questpdf__text_page_number_descriptor__decoration_color__5d18d151(self.target_pointer, color.hex)
        return TextPageNumberDescriptor(result)

    
    
    def decoration_thickness(self, factor: float) -> 'TextPageNumberDescriptor':


        result = questpdf.lib.questpdf__text_page_number_descriptor__decoration_thickness__c7c23c84(self.target_pointer, factor)
        return TextPageNumberDescriptor(result)

    
    
    def decoration_solid(self) -> 'TextPageNumberDescriptor':


        result = questpdf.lib.questpdf__text_page_number_descriptor__decoration_solid__f64746d1(self.target_pointer)
        return TextPageNumberDescriptor(result)

    
    
    def decoration_double(self) -> 'TextPageNumberDescriptor':


        result = questpdf.lib.questpdf__text_page_number_descriptor__decoration_double__41cf8a18(self.target_pointer)
        return TextPageNumberDescriptor(result)

    
    
    def decoration_wavy(self) -> 'TextPageNumberDescriptor':


        result = questpdf.lib.questpdf__text_page_number_descriptor__decoration_wavy__1761acf2(self.target_pointer)
        return TextPageNumberDescriptor(result)

    
    
    def decoration_dotted(self) -> 'TextPageNumberDescriptor':


        result = questpdf.lib.questpdf__text_page_number_descriptor__decoration_dotted__e940537a(self.target_pointer)
        return TextPageNumberDescriptor(result)

    
    
    def decoration_dashed(self) -> 'TextPageNumberDescriptor':


        result = questpdf.lib.questpdf__text_page_number_descriptor__decoration_dashed__a85f7344(self.target_pointer)
        return TextPageNumberDescriptor(result)

    
    
    def thin(self) -> 'TextPageNumberDescriptor':


        result = questpdf.lib.questpdf__text_page_number_descriptor__thin__e9036638(self.target_pointer)
        return TextPageNumberDescriptor(result)

    
    
    def extra_light(self) -> 'TextPageNumberDescriptor':


        result = questpdf.lib.questpdf__text_page_number_descriptor__extra_light__33bbe020(self.target_pointer)
        return TextPageNumberDescriptor(result)

    
    
    def light(self) -> 'TextPageNumberDescriptor':


        result = questpdf.lib.questpdf__text_page_number_descriptor__light__37ef1bc2(self.target_pointer)
        return TextPageNumberDescriptor(result)

    
    
    def normal_weight(self) -> 'TextPageNumberDescriptor':


        result = questpdf.lib.questpdf__text_page_number_descriptor__normal_weight__18d360b3(self.target_pointer)
        return TextPageNumberDescriptor(result)

    
    
    def medium(self) -> 'TextPageNumberDescriptor':


        result = questpdf.lib.questpdf__text_page_number_descriptor__medium__5ef8b80e(self.target_pointer)
        return TextPageNumberDescriptor(result)

    
    
    def semi_bold(self) -> 'TextPageNumberDescriptor':


        result = questpdf.lib.questpdf__text_page_number_descriptor__semi_bold__0b92f7b7(self.target_pointer)
        return TextPageNumberDescriptor(result)

    
    
    def bold(self) -> 'TextPageNumberDescriptor':


        result = questpdf.lib.questpdf__text_page_number_descriptor__bold__0dfa9061(self.target_pointer)
        return TextPageNumberDescriptor(result)

    
    
    def extra_bold(self) -> 'TextPageNumberDescriptor':


        result = questpdf.lib.questpdf__text_page_number_descriptor__extra_bold__c4fbc0a6(self.target_pointer)
        return TextPageNumberDescriptor(result)

    
    
    def black(self) -> 'TextPageNumberDescriptor':


        result = questpdf.lib.questpdf__text_page_number_descriptor__black__0cc8d698(self.target_pointer)
        return TextPageNumberDescriptor(result)

    
    
    def extra_black(self) -> 'TextPageNumberDescriptor':


        result = questpdf.lib.questpdf__text_page_number_descriptor__extra_black__c7698d85(self.target_pointer)
        return TextPageNumberDescriptor(result)

    
    
    def normal_position(self) -> 'TextPageNumberDescriptor':


        result = questpdf.lib.questpdf__text_page_number_descriptor__normal_position__5e5176cb(self.target_pointer)
        return TextPageNumberDescriptor(result)

    
    
    def subscript(self) -> 'TextPageNumberDescriptor':


        result = questpdf.lib.questpdf__text_page_number_descriptor__subscript__db9bd4eb(self.target_pointer)
        return TextPageNumberDescriptor(result)

    
    
    def superscript(self) -> 'TextPageNumberDescriptor':


        result = questpdf.lib.questpdf__text_page_number_descriptor__superscript__a9b46b1e(self.target_pointer)
        return TextPageNumberDescriptor(result)

    
    
    def direction_auto(self) -> 'TextPageNumberDescriptor':


        result = questpdf.lib.questpdf__text_page_number_descriptor__direction_auto__fbed9e71(self.target_pointer)
        return TextPageNumberDescriptor(result)

    
    
    def direction_from_left_to_right(self) -> 'TextPageNumberDescriptor':


        result = questpdf.lib.questpdf__text_page_number_descriptor__direction_from_left_to_right__09e2e3bc(self.target_pointer)
        return TextPageNumberDescriptor(result)

    
    
    def direction_from_right_to_left(self) -> 'TextPageNumberDescriptor':


        result = questpdf.lib.questpdf__text_page_number_descriptor__direction_from_right_to_left__6cc5cb9e(self.target_pointer)
        return TextPageNumberDescriptor(result)

    
    
    def enable_font_feature(self, feature_name: str) -> 'TextPageNumberDescriptor':


        result = questpdf.lib.questpdf__text_page_number_descriptor__enable_font_feature__136a164d(self.target_pointer, questpdf_ffi.new("char[]", feature_name.encode("utf-8")))
        return TextPageNumberDescriptor(result)

    
    
    def disable_font_feature(self, feature_name: str) -> 'TextPageNumberDescriptor':


        result = questpdf.lib.questpdf__text_page_number_descriptor__disable_font_feature__5bd81de9(self.target_pointer, questpdf_ffi.new("char[]", feature_name.encode("utf-8")))
        return TextPageNumberDescriptor(result)






QUESTPDF_CDEF += r"""
    

    
    void* questpdf__text_block_descriptor__align_left__da139fee(void* target);
    
    void* questpdf__text_block_descriptor__align_center__3661d942(void* target);
    
    void* questpdf__text_block_descriptor__align_right__28e79232(void* target);
    
    void* questpdf__text_block_descriptor__justify__f4a5d951(void* target);
    
    void* questpdf__text_block_descriptor__align_start__c97cfc2b(void* target);
    
    void* questpdf__text_block_descriptor__align_end__e0ace0c1(void* target);
    
    void* questpdf__text_block_descriptor__clamp_lines__2a5db05c(void* target, int32_t maxLines, const char* ellipsis);
    
    void* questpdf__text_block_descriptor__paragraph_spacing__6dcddcea(void* target, float value, int32_t unit);
    
    void* questpdf__text_block_descriptor__paragraph_first_line_indentation__5d11cadd(void* target, float value, int32_t unit);
    
    void* questpdf__text_block_descriptor__font_color__a0d06e42(void* descriptor, uint32_t color);
    
    void* questpdf__text_block_descriptor__background_color__5461b453(void* descriptor, uint32_t color);
    
    void* questpdf__text_block_descriptor__font_size__c989487d(void* descriptor, float value);
    
    void* questpdf__text_block_descriptor__line_height__a1b4697a(void* descriptor, void* factor);
    
    void* questpdf__text_block_descriptor__letter_spacing__92f86a26(void* descriptor, float factor);
    
    void* questpdf__text_block_descriptor__word_spacing__1f794add(void* descriptor, float factor);
    
    void* questpdf__text_block_descriptor__italic__4f023aba(void* descriptor, uint8_t value);
    
    void* questpdf__text_block_descriptor__strikethrough__41841206(void* descriptor, uint8_t value);
    
    void* questpdf__text_block_descriptor__underline__2e1ae473(void* descriptor, uint8_t value);
    
    void* questpdf__text_block_descriptor__overline__add25860(void* descriptor, uint8_t value);
    
    void* questpdf__text_block_descriptor__decoration_color__5d18d151(void* descriptor, uint32_t color);
    
    void* questpdf__text_block_descriptor__decoration_thickness__c7c23c84(void* descriptor, float factor);
    
    void* questpdf__text_block_descriptor__decoration_solid__f64746d1(void* descriptor);
    
    void* questpdf__text_block_descriptor__decoration_double__41cf8a18(void* descriptor);
    
    void* questpdf__text_block_descriptor__decoration_wavy__1761acf2(void* descriptor);
    
    void* questpdf__text_block_descriptor__decoration_dotted__e940537a(void* descriptor);
    
    void* questpdf__text_block_descriptor__decoration_dashed__a85f7344(void* descriptor);
    
    void* questpdf__text_block_descriptor__thin__e9036638(void* descriptor);
    
    void* questpdf__text_block_descriptor__extra_light__33bbe020(void* descriptor);
    
    void* questpdf__text_block_descriptor__light__37ef1bc2(void* descriptor);
    
    void* questpdf__text_block_descriptor__normal_weight__18d360b3(void* descriptor);
    
    void* questpdf__text_block_descriptor__medium__5ef8b80e(void* descriptor);
    
    void* questpdf__text_block_descriptor__semi_bold__0b92f7b7(void* descriptor);
    
    void* questpdf__text_block_descriptor__bold__0dfa9061(void* descriptor);
    
    void* questpdf__text_block_descriptor__extra_bold__c4fbc0a6(void* descriptor);
    
    void* questpdf__text_block_descriptor__black__0cc8d698(void* descriptor);
    
    void* questpdf__text_block_descriptor__extra_black__c7698d85(void* descriptor);
    
    void* questpdf__text_block_descriptor__normal_position__5e5176cb(void* descriptor);
    
    void* questpdf__text_block_descriptor__subscript__db9bd4eb(void* descriptor);
    
    void* questpdf__text_block_descriptor__superscript__a9b46b1e(void* descriptor);
    
    void* questpdf__text_block_descriptor__direction_auto__fbed9e71(void* descriptor);
    
    void* questpdf__text_block_descriptor__direction_from_left_to_right__09e2e3bc(void* descriptor);
    
    void* questpdf__text_block_descriptor__direction_from_right_to_left__6cc5cb9e(void* descriptor);
    
    void* questpdf__text_block_descriptor__enable_font_feature__136a164d(void* descriptor, const char* featureName);
    
    void* questpdf__text_block_descriptor__disable_font_feature__5bd81de9(void* descriptor, const char* featureName);
    


"""



class TextBlockDescriptor:
    def __init__(self, target_pointer):
        self.target_pointer = target_pointer
        self._callbacks = []  # prevent GC of callbacks during native calls


    
    
    def align_left(self) -> 'TextBlockDescriptor':


        result = questpdf.lib.questpdf__text_block_descriptor__align_left__da139fee(self.target_pointer)
        return TextBlockDescriptor(result)

    
    
    def align_center(self) -> 'TextBlockDescriptor':


        result = questpdf.lib.questpdf__text_block_descriptor__align_center__3661d942(self.target_pointer)
        return TextBlockDescriptor(result)

    
    
    def align_right(self) -> 'TextBlockDescriptor':


        result = questpdf.lib.questpdf__text_block_descriptor__align_right__28e79232(self.target_pointer)
        return TextBlockDescriptor(result)

    
    
    def justify(self) -> 'TextBlockDescriptor':


        result = questpdf.lib.questpdf__text_block_descriptor__justify__f4a5d951(self.target_pointer)
        return TextBlockDescriptor(result)

    
    
    def align_start(self) -> 'TextBlockDescriptor':


        result = questpdf.lib.questpdf__text_block_descriptor__align_start__c97cfc2b(self.target_pointer)
        return TextBlockDescriptor(result)

    
    
    def align_end(self) -> 'TextBlockDescriptor':


        result = questpdf.lib.questpdf__text_block_descriptor__align_end__e0ace0c1(self.target_pointer)
        return TextBlockDescriptor(result)

    
    
    def clamp_lines(self, max_lines: int, ellipsis: str = '…') -> 'TextBlockDescriptor':


        result = questpdf.lib.questpdf__text_block_descriptor__clamp_lines__2a5db05c(self.target_pointer, max_lines, questpdf_ffi.new("char[]", ellipsis.encode("utf-8")))
        return TextBlockDescriptor(result)

    
    
    def paragraph_spacing(self, value: float, unit: Unit = Unit.point) -> 'TextBlockDescriptor':


        result = questpdf.lib.questpdf__text_block_descriptor__paragraph_spacing__6dcddcea(self.target_pointer, value, unit.value)
        return TextBlockDescriptor(result)

    
    
    def paragraph_first_line_indentation(self, value: float, unit: Unit = Unit.point) -> 'TextBlockDescriptor':


        result = questpdf.lib.questpdf__text_block_descriptor__paragraph_first_line_indentation__5d11cadd(self.target_pointer, value, unit.value)
        return TextBlockDescriptor(result)

    
    
    def font_color(self, color: Color) -> 'TextBlockDescriptor':


        result = questpdf.lib.questpdf__text_block_descriptor__font_color__a0d06e42(self.target_pointer, color.hex)
        return TextBlockDescriptor(result)

    
    
    def background_color(self, color: Color) -> 'TextBlockDescriptor':


        result = questpdf.lib.questpdf__text_block_descriptor__background_color__5461b453(self.target_pointer, color.hex)
        return TextBlockDescriptor(result)

    
    
    def font_size(self, value: float) -> 'TextBlockDescriptor':


        result = questpdf.lib.questpdf__text_block_descriptor__font_size__c989487d(self.target_pointer, value)
        return TextBlockDescriptor(result)

    
    
    def line_height(self, factor: Any) -> 'TextBlockDescriptor':


        result = questpdf.lib.questpdf__text_block_descriptor__line_height__a1b4697a(self.target_pointer, factor)
        return TextBlockDescriptor(result)

    
    
    def letter_spacing(self, factor: float = 0) -> 'TextBlockDescriptor':


        result = questpdf.lib.questpdf__text_block_descriptor__letter_spacing__92f86a26(self.target_pointer, factor)
        return TextBlockDescriptor(result)

    
    
    def word_spacing(self, factor: float = 0) -> 'TextBlockDescriptor':


        result = questpdf.lib.questpdf__text_block_descriptor__word_spacing__1f794add(self.target_pointer, factor)
        return TextBlockDescriptor(result)

    
    
    def italic(self, value: bool = True) -> 'TextBlockDescriptor':


        result = questpdf.lib.questpdf__text_block_descriptor__italic__4f023aba(self.target_pointer, value)
        return TextBlockDescriptor(result)

    
    
    def strikethrough(self, value: bool = True) -> 'TextBlockDescriptor':


        result = questpdf.lib.questpdf__text_block_descriptor__strikethrough__41841206(self.target_pointer, value)
        return TextBlockDescriptor(result)

    
    
    def underline(self, value: bool = True) -> 'TextBlockDescriptor':


        result = questpdf.lib.questpdf__text_block_descriptor__underline__2e1ae473(self.target_pointer, value)
        return TextBlockDescriptor(result)

    
    
    def overline(self, value: bool = True) -> 'TextBlockDescriptor':


        result = questpdf.lib.questpdf__text_block_descriptor__overline__add25860(self.target_pointer, value)
        return TextBlockDescriptor(result)

    
    
    def decoration_color(self, color: Color) -> 'TextBlockDescriptor':


        result = questpdf.lib.questpdf__text_block_descriptor__decoration_color__5d18d151(self.target_pointer, color.hex)
        return TextBlockDescriptor(result)

    
    
    def decoration_thickness(self, factor: float) -> 'TextBlockDescriptor':


        result = questpdf.lib.questpdf__text_block_descriptor__decoration_thickness__c7c23c84(self.target_pointer, factor)
        return TextBlockDescriptor(result)

    
    
    def decoration_solid(self) -> 'TextBlockDescriptor':


        result = questpdf.lib.questpdf__text_block_descriptor__decoration_solid__f64746d1(self.target_pointer)
        return TextBlockDescriptor(result)

    
    
    def decoration_double(self) -> 'TextBlockDescriptor':


        result = questpdf.lib.questpdf__text_block_descriptor__decoration_double__41cf8a18(self.target_pointer)
        return TextBlockDescriptor(result)

    
    
    def decoration_wavy(self) -> 'TextBlockDescriptor':


        result = questpdf.lib.questpdf__text_block_descriptor__decoration_wavy__1761acf2(self.target_pointer)
        return TextBlockDescriptor(result)

    
    
    def decoration_dotted(self) -> 'TextBlockDescriptor':


        result = questpdf.lib.questpdf__text_block_descriptor__decoration_dotted__e940537a(self.target_pointer)
        return TextBlockDescriptor(result)

    
    
    def decoration_dashed(self) -> 'TextBlockDescriptor':


        result = questpdf.lib.questpdf__text_block_descriptor__decoration_dashed__a85f7344(self.target_pointer)
        return TextBlockDescriptor(result)

    
    
    def thin(self) -> 'TextBlockDescriptor':


        result = questpdf.lib.questpdf__text_block_descriptor__thin__e9036638(self.target_pointer)
        return TextBlockDescriptor(result)

    
    
    def extra_light(self) -> 'TextBlockDescriptor':


        result = questpdf.lib.questpdf__text_block_descriptor__extra_light__33bbe020(self.target_pointer)
        return TextBlockDescriptor(result)

    
    
    def light(self) -> 'TextBlockDescriptor':


        result = questpdf.lib.questpdf__text_block_descriptor__light__37ef1bc2(self.target_pointer)
        return TextBlockDescriptor(result)

    
    
    def normal_weight(self) -> 'TextBlockDescriptor':


        result = questpdf.lib.questpdf__text_block_descriptor__normal_weight__18d360b3(self.target_pointer)
        return TextBlockDescriptor(result)

    
    
    def medium(self) -> 'TextBlockDescriptor':


        result = questpdf.lib.questpdf__text_block_descriptor__medium__5ef8b80e(self.target_pointer)
        return TextBlockDescriptor(result)

    
    
    def semi_bold(self) -> 'TextBlockDescriptor':


        result = questpdf.lib.questpdf__text_block_descriptor__semi_bold__0b92f7b7(self.target_pointer)
        return TextBlockDescriptor(result)

    
    
    def bold(self) -> 'TextBlockDescriptor':


        result = questpdf.lib.questpdf__text_block_descriptor__bold__0dfa9061(self.target_pointer)
        return TextBlockDescriptor(result)

    
    
    def extra_bold(self) -> 'TextBlockDescriptor':


        result = questpdf.lib.questpdf__text_block_descriptor__extra_bold__c4fbc0a6(self.target_pointer)
        return TextBlockDescriptor(result)

    
    
    def black(self) -> 'TextBlockDescriptor':


        result = questpdf.lib.questpdf__text_block_descriptor__black__0cc8d698(self.target_pointer)
        return TextBlockDescriptor(result)

    
    
    def extra_black(self) -> 'TextBlockDescriptor':


        result = questpdf.lib.questpdf__text_block_descriptor__extra_black__c7698d85(self.target_pointer)
        return TextBlockDescriptor(result)

    
    
    def normal_position(self) -> 'TextBlockDescriptor':


        result = questpdf.lib.questpdf__text_block_descriptor__normal_position__5e5176cb(self.target_pointer)
        return TextBlockDescriptor(result)

    
    
    def subscript(self) -> 'TextBlockDescriptor':


        result = questpdf.lib.questpdf__text_block_descriptor__subscript__db9bd4eb(self.target_pointer)
        return TextBlockDescriptor(result)

    
    
    def superscript(self) -> 'TextBlockDescriptor':


        result = questpdf.lib.questpdf__text_block_descriptor__superscript__a9b46b1e(self.target_pointer)
        return TextBlockDescriptor(result)

    
    
    def direction_auto(self) -> 'TextBlockDescriptor':


        result = questpdf.lib.questpdf__text_block_descriptor__direction_auto__fbed9e71(self.target_pointer)
        return TextBlockDescriptor(result)

    
    
    def direction_from_left_to_right(self) -> 'TextBlockDescriptor':


        result = questpdf.lib.questpdf__text_block_descriptor__direction_from_left_to_right__09e2e3bc(self.target_pointer)
        return TextBlockDescriptor(result)

    
    
    def direction_from_right_to_left(self) -> 'TextBlockDescriptor':


        result = questpdf.lib.questpdf__text_block_descriptor__direction_from_right_to_left__6cc5cb9e(self.target_pointer)
        return TextBlockDescriptor(result)

    
    
    def enable_font_feature(self, feature_name: str) -> 'TextBlockDescriptor':


        result = questpdf.lib.questpdf__text_block_descriptor__enable_font_feature__136a164d(self.target_pointer, questpdf_ffi.new("char[]", feature_name.encode("utf-8")))
        return TextBlockDescriptor(result)

    
    
    def disable_font_feature(self, feature_name: str) -> 'TextBlockDescriptor':


        result = questpdf.lib.questpdf__text_block_descriptor__disable_font_feature__5bd81de9(self.target_pointer, questpdf_ffi.new("char[]", feature_name.encode("utf-8")))
        return TextBlockDescriptor(result)






QUESTPDF_CDEF += r"""
    

    
    void* questpdf__image_descriptor__use_original_image__d3be84c2(void* target, uint8_t value);
    
    void* questpdf__image_descriptor__with_raster_dpi__78f617ee(void* target, int32_t dpi);
    
    void* questpdf__image_descriptor__with_compression_quality__1665a445(void* target, int32_t quality);
    
    void* questpdf__image_descriptor__fit_width__7b9aa4d6(void* target);
    
    void* questpdf__image_descriptor__fit_height__c888daad(void* target);
    
    void* questpdf__image_descriptor__fit_area__4dbf28f5(void* target);
    
    void* questpdf__image_descriptor__fit_unproportionally__3d7bad76(void* target);
    


"""



class ImageDescriptor:
    def __init__(self, target_pointer):
        self.target_pointer = target_pointer
        self._callbacks = []  # prevent GC of callbacks during native calls


    
    
    def use_original_image(self, value: bool = True) -> 'ImageDescriptor':


        result = questpdf.lib.questpdf__image_descriptor__use_original_image__d3be84c2(self.target_pointer, value)
        return ImageDescriptor(result)

    
    
    def with_raster_dpi(self, dpi: int) -> 'ImageDescriptor':


        result = questpdf.lib.questpdf__image_descriptor__with_raster_dpi__78f617ee(self.target_pointer, dpi)
        return ImageDescriptor(result)

    
    
    def with_compression_quality(self, quality: ImageCompressionQuality) -> 'ImageDescriptor':


        result = questpdf.lib.questpdf__image_descriptor__with_compression_quality__1665a445(self.target_pointer, quality.value)
        return ImageDescriptor(result)

    
    
    def fit_width(self) -> 'ImageDescriptor':


        result = questpdf.lib.questpdf__image_descriptor__fit_width__7b9aa4d6(self.target_pointer)
        return ImageDescriptor(result)

    
    
    def fit_height(self) -> 'ImageDescriptor':


        result = questpdf.lib.questpdf__image_descriptor__fit_height__c888daad(self.target_pointer)
        return ImageDescriptor(result)

    
    
    def fit_area(self) -> 'ImageDescriptor':


        result = questpdf.lib.questpdf__image_descriptor__fit_area__4dbf28f5(self.target_pointer)
        return ImageDescriptor(result)

    
    
    def fit_unproportionally(self) -> 'ImageDescriptor':


        result = questpdf.lib.questpdf__image_descriptor__fit_unproportionally__3d7bad76(self.target_pointer)
        return ImageDescriptor(result)






QUESTPDF_CDEF += r"""
    

    
    void* questpdf__dynamic_image_descriptor__use_original_image__1dabc6b8(void* target, uint8_t value);
    
    void* questpdf__dynamic_image_descriptor__with_raster_dpi__a72018d5(void* target, int32_t dpi);
    
    void* questpdf__dynamic_image_descriptor__with_compression_quality__94465629(void* target, int32_t quality);
    


"""



class DynamicImageDescriptor:
    def __init__(self, target_pointer):
        self.target_pointer = target_pointer
        self._callbacks = []  # prevent GC of callbacks during native calls


    
    
    def use_original_image(self, value: bool = True) -> 'DynamicImageDescriptor':


        result = questpdf.lib.questpdf__dynamic_image_descriptor__use_original_image__1dabc6b8(self.target_pointer, value)
        return DynamicImageDescriptor(result)

    
    
    def with_raster_dpi(self, dpi: int) -> 'DynamicImageDescriptor':


        result = questpdf.lib.questpdf__dynamic_image_descriptor__with_raster_dpi__a72018d5(self.target_pointer, dpi)
        return DynamicImageDescriptor(result)

    
    
    def with_compression_quality(self, quality: ImageCompressionQuality) -> 'DynamicImageDescriptor':


        result = questpdf.lib.questpdf__dynamic_image_descriptor__with_compression_quality__94465629(self.target_pointer, quality.value)
        return DynamicImageDescriptor(result)






QUESTPDF_CDEF += r"""
    

    
    void* questpdf__svg_image_descriptor__fit_width__ae37e277(void* target);
    
    void* questpdf__svg_image_descriptor__fit_height__7e11f169(void* target);
    
    void* questpdf__svg_image_descriptor__fit_area__6abce393(void* target);
    


"""



class SvgImageDescriptor:
    def __init__(self, target_pointer):
        self.target_pointer = target_pointer
        self._callbacks = []  # prevent GC of callbacks during native calls


    
    
    def fit_width(self) -> 'SvgImageDescriptor':


        result = questpdf.lib.questpdf__svg_image_descriptor__fit_width__ae37e277(self.target_pointer)
        return SvgImageDescriptor(result)

    
    
    def fit_height(self) -> 'SvgImageDescriptor':


        result = questpdf.lib.questpdf__svg_image_descriptor__fit_height__7e11f169(self.target_pointer)
        return SvgImageDescriptor(result)

    
    
    def fit_area(self) -> 'SvgImageDescriptor':


        result = questpdf.lib.questpdf__svg_image_descriptor__fit_area__6abce393(self.target_pointer)
        return SvgImageDescriptor(result)






QUESTPDF_CDEF += r"""
    

    
    void questpdf__page_descriptor__size__cd4bd97a(void* target, float width, float height, int32_t unit);
    
    void questpdf__page_descriptor__size__593af4d7(void* target, void* pageSize);
    
    void questpdf__page_descriptor__continuous_size__ae1c9536(void* target, float width, int32_t unit);
    
    void questpdf__page_descriptor__min_size__8216ba5f(void* target, void* pageSize);
    
    void questpdf__page_descriptor__max_size__02a75d07(void* target, void* pageSize);
    
    void questpdf__page_descriptor__margin_left__4c6b8a4c(void* target, float value, int32_t unit);
    
    void questpdf__page_descriptor__margin_right__de714424(void* target, float value, int32_t unit);
    
    void questpdf__page_descriptor__margin_top__51f73aad(void* target, float value, int32_t unit);
    
    void questpdf__page_descriptor__margin_bottom__d37c07b0(void* target, float value, int32_t unit);
    
    void questpdf__page_descriptor__margin_vertical__97f4d868(void* target, float value, int32_t unit);
    
    void questpdf__page_descriptor__margin_horizontal__bafb50fd(void* target, float value, int32_t unit);
    
    void questpdf__page_descriptor__margin__35d8c8b5(void* target, float value, int32_t unit);
    
    void questpdf__page_descriptor__content_from_left_to_right__6bcc64b0(void* target);
    
    void questpdf__page_descriptor__content_from_right_to_left__dbce6933(void* target);
    
    void questpdf__page_descriptor__page_color__7ab89cfe(void* target, uint32_t color);
    
    void* questpdf__page_descriptor__background__8048fed3(void* target);
    
    void* questpdf__page_descriptor__foreground__b6d36c4d(void* target);
    
    void* questpdf__page_descriptor__header__392dd2be(void* target);
    
    void* questpdf__page_descriptor__content__af503598(void* target);
    
    void* questpdf__page_descriptor__footer__eb07832c(void* target);
    


"""



class PageDescriptor:
    def __init__(self, target_pointer):
        self.target_pointer = target_pointer
        self._callbacks = []  # prevent GC of callbacks during native calls


    
    
    def _size_float_float_unit(self, width: float, height: float, unit: Unit = Unit.point) -> None:


        result = questpdf.lib.questpdf__page_descriptor__size__cd4bd97a(self.target_pointer, width, height, unit.value)
        

    
    
    def _size_page_size(self, page_size: PageSize) -> None:


        result = questpdf.lib.questpdf__page_descriptor__size__593af4d7(self.target_pointer, page_size)
        

    
    
    def continuous_size(self, width: float, unit: Unit = Unit.point) -> None:


        result = questpdf.lib.questpdf__page_descriptor__continuous_size__ae1c9536(self.target_pointer, width, unit.value)
        

    
    
    def min_size(self, page_size: PageSize) -> None:


        result = questpdf.lib.questpdf__page_descriptor__min_size__8216ba5f(self.target_pointer, page_size)
        

    
    
    def max_size(self, page_size: PageSize) -> None:


        result = questpdf.lib.questpdf__page_descriptor__max_size__02a75d07(self.target_pointer, page_size)
        

    
    
    def margin_left(self, value: float, unit: Unit = Unit.point) -> None:


        result = questpdf.lib.questpdf__page_descriptor__margin_left__4c6b8a4c(self.target_pointer, value, unit.value)
        

    
    
    def margin_right(self, value: float, unit: Unit = Unit.point) -> None:


        result = questpdf.lib.questpdf__page_descriptor__margin_right__de714424(self.target_pointer, value, unit.value)
        

    
    
    def margin_top(self, value: float, unit: Unit = Unit.point) -> None:


        result = questpdf.lib.questpdf__page_descriptor__margin_top__51f73aad(self.target_pointer, value, unit.value)
        

    
    
    def margin_bottom(self, value: float, unit: Unit = Unit.point) -> None:


        result = questpdf.lib.questpdf__page_descriptor__margin_bottom__d37c07b0(self.target_pointer, value, unit.value)
        

    
    
    def margin_vertical(self, value: float, unit: Unit = Unit.point) -> None:


        result = questpdf.lib.questpdf__page_descriptor__margin_vertical__97f4d868(self.target_pointer, value, unit.value)
        

    
    
    def margin_horizontal(self, value: float, unit: Unit = Unit.point) -> None:


        result = questpdf.lib.questpdf__page_descriptor__margin_horizontal__bafb50fd(self.target_pointer, value, unit.value)
        

    
    
    def margin(self, value: float, unit: Unit = Unit.point) -> None:


        result = questpdf.lib.questpdf__page_descriptor__margin__35d8c8b5(self.target_pointer, value, unit.value)
        

    
    
    def content_from_left_to_right(self) -> None:


        result = questpdf.lib.questpdf__page_descriptor__content_from_left_to_right__6bcc64b0(self.target_pointer)
        

    
    
    def content_from_right_to_left(self) -> None:


        result = questpdf.lib.questpdf__page_descriptor__content_from_right_to_left__dbce6933(self.target_pointer)
        

    
    
    def page_color(self, color: Color) -> None:


        result = questpdf.lib.questpdf__page_descriptor__page_color__7ab89cfe(self.target_pointer, color.hex)
        

    
    
    def background(self) -> 'Container':


        result = questpdf.lib.questpdf__page_descriptor__background__8048fed3(self.target_pointer)
        return Container(result)

    
    
    def foreground(self) -> 'Container':


        result = questpdf.lib.questpdf__page_descriptor__foreground__b6d36c4d(self.target_pointer)
        return Container(result)

    
    
    def header(self) -> 'Container':


        result = questpdf.lib.questpdf__page_descriptor__header__392dd2be(self.target_pointer)
        return Container(result)

    
    
    def content(self) -> 'Container':


        result = questpdf.lib.questpdf__page_descriptor__content__af503598(self.target_pointer)
        return Container(result)

    
    
    def footer(self) -> 'Container':


        result = questpdf.lib.questpdf__page_descriptor__footer__eb07832c(self.target_pointer)
        return Container(result)






QUESTPDF_CDEF += r"""
    
    typedef void (*column_descriptor_callback_24d6ceed)(void*);
    
    typedef void (*decoration_descriptor_callback_0b39c58e)(void*);
    
    typedef void (*container_callback_971e7b54)(void*);
    
    typedef void (*container_callback_a33b5f9b)(void*);
    
    typedef void (*inlined_descriptor_callback_33b27c8d)(void*);
    
    typedef void (*layers_descriptor_callback_03ce5bdd)(void*);
    
    typedef void (*multi_column_descriptor_callback_193479d6)(void*);
    
    typedef void (*row_descriptor_callback_39fce557)(void*);
    
    typedef void (*table_descriptor_callback_d49da987)(void*);
    
    typedef void (*text_descriptor_callback_357e362f)(void*);
    

    
    void* questpdf__container__align_left__68bfdc67(void* element);
    
    void* questpdf__container__align_center__4fb1e0d1(void* element);
    
    void* questpdf__container__align_right__a1c1a1bf(void* element);
    
    void* questpdf__container__align_top__f275ca95(void* element);
    
    void* questpdf__container__align_middle__95fef9e8(void* element);
    
    void* questpdf__container__align_bottom__d33d0520(void* element);
    
    void questpdf__container__column__24d6ceed(void* element, column_descriptor_callback_24d6ceed handler);
    
    void* questpdf__container__width__a346e20f(void* element, float value, int32_t unit);
    
    void* questpdf__container__min_width__c00f1915(void* element, float value, int32_t unit);
    
    void* questpdf__container__max_width__7e85a057(void* element, float value, int32_t unit);
    
    void* questpdf__container__height__36ac3a02(void* element, float value, int32_t unit);
    
    void* questpdf__container__min_height__58cc06b0(void* element, float value, int32_t unit);
    
    void* questpdf__container__max_height__0b76e199(void* element, float value, int32_t unit);
    
    void* questpdf__container__content_from_left_to_right__191523c1(void* element);
    
    void* questpdf__container__content_from_right_to_left__a31dbd9d(void* element);
    
    void* questpdf__container__debug_area__a69b9c65(void* parent, const char* text, uint32_t color);
    
    void* questpdf__container__debug_pointer__9d669879(void* parent, const char* label);
    
    void questpdf__container__decoration__0b39c58e(void* element, decoration_descriptor_callback_0b39c58e handler);
    
    void* questpdf__container__aspect_ratio__fd5bc0dc(void* element, float ratio, int32_t option);
    
    void questpdf__container__placeholder__a560192f(void* element, const char* text);
    
    void* questpdf__container__show_once__c6224013(void* element);
    
    void* questpdf__container__skip_once__b3d4c7bf(void* element);
    
    void* questpdf__container__show_entire__16629c88(void* element);
    
    void* questpdf__container__ensure_space__0cbedd6a(void* element, float minHeight);
    
    void* questpdf__container__prevent_page_break__2e3cab6a(void* element);
    
    void questpdf__container__page_break__4287fb55(void* element);
    
    void* questpdf__container__container__be126adc(void* element);
    
    void* questpdf__container__hyperlink__40aee55c(void* element, const char* url);
    
    void* questpdf__container__section__b2687119(void* element, const char* sectionName);
    
    void* questpdf__container__section_link__d27b4828(void* element, const char* sectionName);
    
    void* questpdf__container__show_if__da52e306(void* element, uint8_t condition);
    
    void* questpdf__container__unconstrained__a43107f6(void* element);
    
    void* questpdf__container__stop_paging__81b05f34(void* element);
    
    void* questpdf__container__scale_to_fit__bb0b4e57(void* element);
    
    void* questpdf__container__repeat__e198bc84(void* element);
    
    void questpdf__container__lazy__971e7b54(void* element, container_callback_971e7b54 contentBuilder);
    
    void questpdf__container__lazy_with_cache__a33b5f9b(void* element, container_callback_a33b5f9b contentBuilder);
    
    void* questpdf__container__z_index__9cd9a32e(void* element, int32_t indexValue);
    
    void* questpdf__container__capture_content_position__845fb313(void* element, const char* id);
    
    void* questpdf__container__extend__291e835a(void* element);
    
    void* questpdf__container__extend_vertical__e63e1d72(void* element);
    
    void* questpdf__container__extend_horizontal__c6d6d128(void* element);
    
    void* questpdf__container__image__9155d14a(void* parent, const char* filePath);
    
    void* questpdf__container__image__ccf976d1(void* parent, void* image);
    
    void questpdf__container__inlined__33b27c8d(void* element, inlined_descriptor_callback_33b27c8d handler);
    
    void questpdf__container__layers__03ce5bdd(void* element, layers_descriptor_callback_03ce5bdd handler);
    
    void* questpdf__container__line_vertical__ab97b857(void* element, float thickness, int32_t unit);
    
    void* questpdf__container__line_horizontal__a6f7f11f(void* element, float thickness, int32_t unit);
    
    void questpdf__container__multi_column__193479d6(void* element, multi_column_descriptor_callback_193479d6 handler);
    
    void* questpdf__container__padding__5daecb7e(void* element, float value, int32_t unit);
    
    void* questpdf__container__padding_horizontal__7a6b255d(void* element, float value, int32_t unit);
    
    void* questpdf__container__padding_vertical__91122aaa(void* element, float value, int32_t unit);
    
    void* questpdf__container__padding_top__de3b7b3b(void* element, float value, int32_t unit);
    
    void* questpdf__container__padding_bottom__74ad0a7b(void* element, float value, int32_t unit);
    
    void* questpdf__container__padding_left__103ee738(void* element, float value, int32_t unit);
    
    void* questpdf__container__padding_right__89d1cf61(void* element, float value, int32_t unit);
    
    void* questpdf__container__rotate_left__c5193e66(void* element);
    
    void* questpdf__container__rotate_right__004c9c52(void* element);
    
    void* questpdf__container__rotate__c33f62ac(void* element, float angle);
    
    void questpdf__container__row__39fce557(void* element, row_descriptor_callback_39fce557 handler);
    
    void* questpdf__container__scale__05521931(void* element, float factor);
    
    void* questpdf__container__scale_horizontal__14d1a9be(void* element, float factor);
    
    void* questpdf__container__scale_vertical__5bc8a8a5(void* element, float factor);
    
    void* questpdf__container__flip_horizontal__744e4fe9(void* element);
    
    void* questpdf__container__flip_vertical__a91487f3(void* element);
    
    void* questpdf__container__flip_over__ce1ff345(void* element);
    
    void* questpdf__container__shrink__4221b85b(void* element);
    
    void* questpdf__container__shrink_vertical__e5042c3c(void* element);
    
    void* questpdf__container__shrink_horizontal__588cfd0f(void* element);
    
    void* questpdf__container__border__a6712928(void* element, float all, uint32_t color);
    
    void* questpdf__container__background__68f98b81(void* element, uint32_t color);
    
    void* questpdf__container__border__17f3b5e4(void* element, float value, int32_t unit);
    
    void* questpdf__container__border_vertical__7922384b(void* element, float value, int32_t unit);
    
    void* questpdf__container__border_horizontal__34913f34(void* element, float value, int32_t unit);
    
    void* questpdf__container__border_left__803ed1e6(void* element, float value, int32_t unit);
    
    void* questpdf__container__border_right__de8ca6bf(void* element, float value, int32_t unit);
    
    void* questpdf__container__border_top__c469b91f(void* element, float value, int32_t unit);
    
    void* questpdf__container__border_bottom__59b8a019(void* element, float value, int32_t unit);
    
    void* questpdf__container__corner_radius__bf7cb39f(void* element, float value, int32_t unit);
    
    void* questpdf__container__corner_radius_top_left__41d08c72(void* element, float value, int32_t unit);
    
    void* questpdf__container__corner_radius_top_right__1497678a(void* element, float value, int32_t unit);
    
    void* questpdf__container__corner_radius_bottom_left__3a8d234a(void* element, float value, int32_t unit);
    
    void* questpdf__container__corner_radius_bottom_right__b07c1d8d(void* element, float value, int32_t unit);
    
    void* questpdf__container__border_color__2a24bda0(void* element, uint32_t color);
    
    void* questpdf__container__border_alignment_outside__ce5e63fa(void* element);
    
    void* questpdf__container__border_alignment_middle__66a27445(void* element);
    
    void* questpdf__container__border_alignment_inside__8cef56b1(void* element);
    
    void* questpdf__container__svg__f547d46e(void* container, const char* svg);
    
    void* questpdf__container__svg__b1de06e3(void* parent, void* image);
    
    void questpdf__container__table__d49da987(void* element, table_descriptor_callback_d49da987 handler);
    
    void questpdf__container__text__357e362f(void* element, text_descriptor_callback_357e362f content);
    
    void* questpdf__container__text__3f6b5b07(void* container, const char* text);
    
    void* questpdf__container__translate_x__351baebe(void* element, float value, int32_t unit);
    
    void* questpdf__container__translate_y__d99602db(void* element, float value, int32_t unit);
    

    void* questpdf__container__background_linear_gradient(void* target, float angle, uint32_t* colors, int colorsLength);
    void* questpdf__container__border_linear_gradient(void* target, float angle, uint32_t* colors, int colorsLength);
    void* questpdf__container__shadow(void* target, float blur, uint32_t color, float offsetX, float offsetY, float spread);

    typedef struct {
        float AvailableSpaceWidth;
        float AvailableSpaceHeight;
        int ImageSizeWidth;
        int ImageSizeHeight;
        int Dpi;
    } DynamicImageSourcePayload;

    typedef struct {
        float Width;
        float Height;
    } DynamicSvgSourcePayload;

    void* questpdf__container__image_bytes(void* target, Buffer data);
    void* questpdf__container__image_dynamic(void* target, Buffer (*source)(DynamicImageSourcePayload));
    void questpdf__container__svg_dynamic(void* target, char* (*source)(DynamicSvgSourcePayload));
    
"""

class DynamicImagePayload:
    __slots__ = ('available_space_width', 'available_space_height', 'image_size_width', 'image_size_height', 'dpi')

    def __init__(self, c_payload):
        self.available_space_width: float = c_payload.AvailableSpaceWidth
        self.available_space_height: float = c_payload.AvailableSpaceHeight
        self.image_size_width: int = c_payload.ImageSizeWidth
        self.image_size_height: int = c_payload.ImageSizeHeight
        self.dpi: int = c_payload.Dpi


class DynamicSvgPayload:
    __slots__ = ('width', 'height')
    
    def __init__(self, c_payload):
        self.width: float = c_payload.Width
        self.height: float = c_payload.Height

class Container:
    def __init__(self, target_pointer):
        self.target_pointer = target_pointer
        self._callbacks = []  # prevent GC of callbacks during native calls


    
    
    def align_left(self) -> 'Container':


        result = questpdf.lib.questpdf__container__align_left__68bfdc67(self.target_pointer)
        return Container(result)

    
    
    def align_center(self) -> 'Container':


        result = questpdf.lib.questpdf__container__align_center__4fb1e0d1(self.target_pointer)
        return Container(result)

    
    
    def align_right(self) -> 'Container':


        result = questpdf.lib.questpdf__container__align_right__a1c1a1bf(self.target_pointer)
        return Container(result)

    
    
    def align_top(self) -> 'Container':


        result = questpdf.lib.questpdf__container__align_top__f275ca95(self.target_pointer)
        return Container(result)

    
    
    def align_middle(self) -> 'Container':


        result = questpdf.lib.questpdf__container__align_middle__95fef9e8(self.target_pointer)
        return Container(result)

    
    
    def align_bottom(self) -> 'Container':


        result = questpdf.lib.questpdf__container__align_bottom__d33d0520(self.target_pointer)
        return Container(result)

    
    
    def column(self, handler: Callable[['ColumnDescriptor'], Any]) -> None:

        @questpdf_ffi.callback("void(void*)")
        def _internal_handler_handler(handler_pointer):
            handler_object = ColumnDescriptor(handler_pointer)
            handler(handler_object)

        self._callbacks.append(_internal_handler_handler)


        result = questpdf.lib.questpdf__container__column__24d6ceed(self.target_pointer, _internal_handler_handler)
        

    
    
    def width(self, value: float, unit: Unit = Unit.point) -> 'Container':


        result = questpdf.lib.questpdf__container__width__a346e20f(self.target_pointer, value, unit.value)
        return Container(result)

    
    
    def min_width(self, value: float, unit: Unit = Unit.point) -> 'Container':


        result = questpdf.lib.questpdf__container__min_width__c00f1915(self.target_pointer, value, unit.value)
        return Container(result)

    
    
    def max_width(self, value: float, unit: Unit = Unit.point) -> 'Container':


        result = questpdf.lib.questpdf__container__max_width__7e85a057(self.target_pointer, value, unit.value)
        return Container(result)

    
    
    def height(self, value: float, unit: Unit = Unit.point) -> 'Container':


        result = questpdf.lib.questpdf__container__height__36ac3a02(self.target_pointer, value, unit.value)
        return Container(result)

    
    
    def min_height(self, value: float, unit: Unit = Unit.point) -> 'Container':


        result = questpdf.lib.questpdf__container__min_height__58cc06b0(self.target_pointer, value, unit.value)
        return Container(result)

    
    
    def max_height(self, value: float, unit: Unit = Unit.point) -> 'Container':


        result = questpdf.lib.questpdf__container__max_height__0b76e199(self.target_pointer, value, unit.value)
        return Container(result)

    
    
    def content_from_left_to_right(self) -> 'Container':


        result = questpdf.lib.questpdf__container__content_from_left_to_right__191523c1(self.target_pointer)
        return Container(result)

    
    
    def content_from_right_to_left(self) -> 'Container':


        result = questpdf.lib.questpdf__container__content_from_right_to_left__a31dbd9d(self.target_pointer)
        return Container(result)

    
    
    def debug_area(self, text: str = None, color: Color = None) -> 'Container':


        result = questpdf.lib.questpdf__container__debug_area__a69b9c65(self.target_pointer, questpdf_ffi.new("char[]", text.encode("utf-8")), color.hex)
        return Container(result)

    
    
    def debug_pointer(self, label: str) -> 'Container':


        result = questpdf.lib.questpdf__container__debug_pointer__9d669879(self.target_pointer, questpdf_ffi.new("char[]", label.encode("utf-8")))
        return Container(result)

    
    
    def decoration(self, handler: Callable[['DecorationDescriptor'], Any]) -> None:

        @questpdf_ffi.callback("void(void*)")
        def _internal_handler_handler(handler_pointer):
            handler_object = DecorationDescriptor(handler_pointer)
            handler(handler_object)

        self._callbacks.append(_internal_handler_handler)


        result = questpdf.lib.questpdf__container__decoration__0b39c58e(self.target_pointer, _internal_handler_handler)
        

    
    
    def aspect_ratio(self, ratio: float, option: AspectRatioOption = AspectRatioOption.fit_width) -> 'Container':


        result = questpdf.lib.questpdf__container__aspect_ratio__fd5bc0dc(self.target_pointer, ratio, option.value)
        return Container(result)

    
    
    def placeholder(self, text: str = None) -> None:


        result = questpdf.lib.questpdf__container__placeholder__a560192f(self.target_pointer, questpdf_ffi.new("char[]", text.encode("utf-8")))
        

    
    
    def show_once(self) -> 'Container':


        result = questpdf.lib.questpdf__container__show_once__c6224013(self.target_pointer)
        return Container(result)

    
    
    def skip_once(self) -> 'Container':


        result = questpdf.lib.questpdf__container__skip_once__b3d4c7bf(self.target_pointer)
        return Container(result)

    
    
    def show_entire(self) -> 'Container':


        result = questpdf.lib.questpdf__container__show_entire__16629c88(self.target_pointer)
        return Container(result)

    
    
    def ensure_space(self, min_height: float = 150) -> 'Container':


        result = questpdf.lib.questpdf__container__ensure_space__0cbedd6a(self.target_pointer, min_height)
        return Container(result)

    
    
    def prevent_page_break(self) -> 'Container':


        result = questpdf.lib.questpdf__container__prevent_page_break__2e3cab6a(self.target_pointer)
        return Container(result)

    
    
    def page_break(self) -> None:


        result = questpdf.lib.questpdf__container__page_break__4287fb55(self.target_pointer)
        

    
    
    def container(self) -> 'Container':


        result = questpdf.lib.questpdf__container__container__be126adc(self.target_pointer)
        return Container(result)

    
    
    def hyperlink(self, url: str) -> 'Container':


        result = questpdf.lib.questpdf__container__hyperlink__40aee55c(self.target_pointer, questpdf_ffi.new("char[]", url.encode("utf-8")))
        return Container(result)

    
    
    def section(self, section_name: str) -> 'Container':


        result = questpdf.lib.questpdf__container__section__b2687119(self.target_pointer, questpdf_ffi.new("char[]", section_name.encode("utf-8")))
        return Container(result)

    
    
    def section_link(self, section_name: str) -> 'Container':


        result = questpdf.lib.questpdf__container__section_link__d27b4828(self.target_pointer, questpdf_ffi.new("char[]", section_name.encode("utf-8")))
        return Container(result)

    
    
    def show_if(self, condition: bool) -> 'Container':


        result = questpdf.lib.questpdf__container__show_if__da52e306(self.target_pointer, condition)
        return Container(result)

    
    
    def unconstrained(self) -> 'Container':


        result = questpdf.lib.questpdf__container__unconstrained__a43107f6(self.target_pointer)
        return Container(result)

    
    
    def stop_paging(self) -> 'Container':


        result = questpdf.lib.questpdf__container__stop_paging__81b05f34(self.target_pointer)
        return Container(result)

    
    
    def scale_to_fit(self) -> 'Container':


        result = questpdf.lib.questpdf__container__scale_to_fit__bb0b4e57(self.target_pointer)
        return Container(result)

    
    
    def repeat(self) -> 'Container':


        result = questpdf.lib.questpdf__container__repeat__e198bc84(self.target_pointer)
        return Container(result)

    
    
    def lazy(self, content_builder: Callable[['Container'], Any]) -> None:

        @questpdf_ffi.callback("void(void*)")
        def _internal_content_builder_handler(content_builder_pointer):
            content_builder_object = Container(content_builder_pointer)
            content_builder(content_builder_object)

        self._callbacks.append(_internal_content_builder_handler)


        result = questpdf.lib.questpdf__container__lazy__971e7b54(self.target_pointer, _internal_content_builder_handler)
        

    
    
    def lazy_with_cache(self, content_builder: Callable[['Container'], Any]) -> None:

        @questpdf_ffi.callback("void(void*)")
        def _internal_content_builder_handler(content_builder_pointer):
            content_builder_object = Container(content_builder_pointer)
            content_builder(content_builder_object)

        self._callbacks.append(_internal_content_builder_handler)


        result = questpdf.lib.questpdf__container__lazy_with_cache__a33b5f9b(self.target_pointer, _internal_content_builder_handler)
        

    
    
    def z_index(self, index_value: int) -> 'Container':


        result = questpdf.lib.questpdf__container__z_index__9cd9a32e(self.target_pointer, index_value)
        return Container(result)

    
    
    def capture_content_position(self, id: str) -> 'Container':


        result = questpdf.lib.questpdf__container__capture_content_position__845fb313(self.target_pointer, questpdf_ffi.new("char[]", id.encode("utf-8")))
        return Container(result)

    
    
    def extend(self) -> 'Container':


        result = questpdf.lib.questpdf__container__extend__291e835a(self.target_pointer)
        return Container(result)

    
    
    def extend_vertical(self) -> 'Container':


        result = questpdf.lib.questpdf__container__extend_vertical__e63e1d72(self.target_pointer)
        return Container(result)

    
    
    def extend_horizontal(self) -> 'Container':


        result = questpdf.lib.questpdf__container__extend_horizontal__c6d6d128(self.target_pointer)
        return Container(result)

    
    
    def _image_string(self, file_path: str) -> 'ImageDescriptor':


        result = questpdf.lib.questpdf__container__image__9155d14a(self.target_pointer, questpdf_ffi.new("char[]", file_path.encode("utf-8")))
        return ImageDescriptor(result)

    
    
    def _image_image(self, image: Image) -> 'ImageDescriptor':


        result = questpdf.lib.questpdf__container__image__ccf976d1(self.target_pointer, image)
        return ImageDescriptor(result)

    
    
    def inlined(self, handler: Callable[['InlinedDescriptor'], Any]) -> None:

        @questpdf_ffi.callback("void(void*)")
        def _internal_handler_handler(handler_pointer):
            handler_object = InlinedDescriptor(handler_pointer)
            handler(handler_object)

        self._callbacks.append(_internal_handler_handler)


        result = questpdf.lib.questpdf__container__inlined__33b27c8d(self.target_pointer, _internal_handler_handler)
        

    
    
    def layers(self, handler: Callable[['LayersDescriptor'], Any]) -> None:

        @questpdf_ffi.callback("void(void*)")
        def _internal_handler_handler(handler_pointer):
            handler_object = LayersDescriptor(handler_pointer)
            handler(handler_object)

        self._callbacks.append(_internal_handler_handler)


        result = questpdf.lib.questpdf__container__layers__03ce5bdd(self.target_pointer, _internal_handler_handler)
        

    
    
    def line_vertical(self, thickness: float, unit: Unit = Unit.point) -> 'LineDescriptor':


        result = questpdf.lib.questpdf__container__line_vertical__ab97b857(self.target_pointer, thickness, unit.value)
        return LineDescriptor(result)

    
    
    def line_horizontal(self, thickness: float, unit: Unit = Unit.point) -> 'LineDescriptor':


        result = questpdf.lib.questpdf__container__line_horizontal__a6f7f11f(self.target_pointer, thickness, unit.value)
        return LineDescriptor(result)

    
    
    def multi_column(self, handler: Callable[['MultiColumnDescriptor'], Any]) -> None:

        @questpdf_ffi.callback("void(void*)")
        def _internal_handler_handler(handler_pointer):
            handler_object = MultiColumnDescriptor(handler_pointer)
            handler(handler_object)

        self._callbacks.append(_internal_handler_handler)


        result = questpdf.lib.questpdf__container__multi_column__193479d6(self.target_pointer, _internal_handler_handler)
        

    
    
    def padding(self, value: float, unit: Unit = Unit.point) -> 'Container':


        result = questpdf.lib.questpdf__container__padding__5daecb7e(self.target_pointer, value, unit.value)
        return Container(result)

    
    
    def padding_horizontal(self, value: float, unit: Unit = Unit.point) -> 'Container':


        result = questpdf.lib.questpdf__container__padding_horizontal__7a6b255d(self.target_pointer, value, unit.value)
        return Container(result)

    
    
    def padding_vertical(self, value: float, unit: Unit = Unit.point) -> 'Container':


        result = questpdf.lib.questpdf__container__padding_vertical__91122aaa(self.target_pointer, value, unit.value)
        return Container(result)

    
    
    def padding_top(self, value: float, unit: Unit = Unit.point) -> 'Container':


        result = questpdf.lib.questpdf__container__padding_top__de3b7b3b(self.target_pointer, value, unit.value)
        return Container(result)

    
    
    def padding_bottom(self, value: float, unit: Unit = Unit.point) -> 'Container':


        result = questpdf.lib.questpdf__container__padding_bottom__74ad0a7b(self.target_pointer, value, unit.value)
        return Container(result)

    
    
    def padding_left(self, value: float, unit: Unit = Unit.point) -> 'Container':


        result = questpdf.lib.questpdf__container__padding_left__103ee738(self.target_pointer, value, unit.value)
        return Container(result)

    
    
    def padding_right(self, value: float, unit: Unit = Unit.point) -> 'Container':


        result = questpdf.lib.questpdf__container__padding_right__89d1cf61(self.target_pointer, value, unit.value)
        return Container(result)

    
    
    def rotate_left(self) -> 'Container':


        result = questpdf.lib.questpdf__container__rotate_left__c5193e66(self.target_pointer)
        return Container(result)

    
    
    def rotate_right(self) -> 'Container':


        result = questpdf.lib.questpdf__container__rotate_right__004c9c52(self.target_pointer)
        return Container(result)

    
    
    def rotate(self, angle: float) -> 'Container':


        result = questpdf.lib.questpdf__container__rotate__c33f62ac(self.target_pointer, angle)
        return Container(result)

    
    
    def row(self, handler: Callable[['RowDescriptor'], Any]) -> None:

        @questpdf_ffi.callback("void(void*)")
        def _internal_handler_handler(handler_pointer):
            handler_object = RowDescriptor(handler_pointer)
            handler(handler_object)

        self._callbacks.append(_internal_handler_handler)


        result = questpdf.lib.questpdf__container__row__39fce557(self.target_pointer, _internal_handler_handler)
        

    
    
    def scale(self, factor: float) -> 'Container':


        result = questpdf.lib.questpdf__container__scale__05521931(self.target_pointer, factor)
        return Container(result)

    
    
    def scale_horizontal(self, factor: float) -> 'Container':


        result = questpdf.lib.questpdf__container__scale_horizontal__14d1a9be(self.target_pointer, factor)
        return Container(result)

    
    
    def scale_vertical(self, factor: float) -> 'Container':


        result = questpdf.lib.questpdf__container__scale_vertical__5bc8a8a5(self.target_pointer, factor)
        return Container(result)

    
    
    def flip_horizontal(self) -> 'Container':


        result = questpdf.lib.questpdf__container__flip_horizontal__744e4fe9(self.target_pointer)
        return Container(result)

    
    
    def flip_vertical(self) -> 'Container':


        result = questpdf.lib.questpdf__container__flip_vertical__a91487f3(self.target_pointer)
        return Container(result)

    
    
    def flip_over(self) -> 'Container':


        result = questpdf.lib.questpdf__container__flip_over__ce1ff345(self.target_pointer)
        return Container(result)

    
    
    def shrink(self) -> 'Container':


        result = questpdf.lib.questpdf__container__shrink__4221b85b(self.target_pointer)
        return Container(result)

    
    
    def shrink_vertical(self) -> 'Container':


        result = questpdf.lib.questpdf__container__shrink_vertical__e5042c3c(self.target_pointer)
        return Container(result)

    
    
    def shrink_horizontal(self) -> 'Container':


        result = questpdf.lib.questpdf__container__shrink_horizontal__588cfd0f(self.target_pointer)
        return Container(result)

    
    
    def _border_float_color(self, all: float, color: Color) -> 'Container':


        result = questpdf.lib.questpdf__container__border__a6712928(self.target_pointer, all, color.hex)
        return Container(result)

    
    
    def background(self, color: Color) -> 'Container':


        result = questpdf.lib.questpdf__container__background__68f98b81(self.target_pointer, color.hex)
        return Container(result)

    
    
    def _border_float_unit(self, value: float, unit: Unit = Unit.point) -> 'Container':


        result = questpdf.lib.questpdf__container__border__17f3b5e4(self.target_pointer, value, unit.value)
        return Container(result)

    
    
    def border_vertical(self, value: float, unit: Unit = Unit.point) -> 'Container':


        result = questpdf.lib.questpdf__container__border_vertical__7922384b(self.target_pointer, value, unit.value)
        return Container(result)

    
    
    def border_horizontal(self, value: float, unit: Unit = Unit.point) -> 'Container':


        result = questpdf.lib.questpdf__container__border_horizontal__34913f34(self.target_pointer, value, unit.value)
        return Container(result)

    
    
    def border_left(self, value: float, unit: Unit = Unit.point) -> 'Container':


        result = questpdf.lib.questpdf__container__border_left__803ed1e6(self.target_pointer, value, unit.value)
        return Container(result)

    
    
    def border_right(self, value: float, unit: Unit = Unit.point) -> 'Container':


        result = questpdf.lib.questpdf__container__border_right__de8ca6bf(self.target_pointer, value, unit.value)
        return Container(result)

    
    
    def border_top(self, value: float, unit: Unit = Unit.point) -> 'Container':


        result = questpdf.lib.questpdf__container__border_top__c469b91f(self.target_pointer, value, unit.value)
        return Container(result)

    
    
    def border_bottom(self, value: float, unit: Unit = Unit.point) -> 'Container':


        result = questpdf.lib.questpdf__container__border_bottom__59b8a019(self.target_pointer, value, unit.value)
        return Container(result)

    
    
    def corner_radius(self, value: float = 0, unit: Unit = Unit.point) -> 'Container':


        result = questpdf.lib.questpdf__container__corner_radius__bf7cb39f(self.target_pointer, value, unit.value)
        return Container(result)

    
    
    def corner_radius_top_left(self, value: float = 0, unit: Unit = Unit.point) -> 'Container':


        result = questpdf.lib.questpdf__container__corner_radius_top_left__41d08c72(self.target_pointer, value, unit.value)
        return Container(result)

    
    
    def corner_radius_top_right(self, value: float = 0, unit: Unit = Unit.point) -> 'Container':


        result = questpdf.lib.questpdf__container__corner_radius_top_right__1497678a(self.target_pointer, value, unit.value)
        return Container(result)

    
    
    def corner_radius_bottom_left(self, value: float = 0, unit: Unit = Unit.point) -> 'Container':


        result = questpdf.lib.questpdf__container__corner_radius_bottom_left__3a8d234a(self.target_pointer, value, unit.value)
        return Container(result)

    
    
    def corner_radius_bottom_right(self, value: float = 0, unit: Unit = Unit.point) -> 'Container':


        result = questpdf.lib.questpdf__container__corner_radius_bottom_right__b07c1d8d(self.target_pointer, value, unit.value)
        return Container(result)

    
    
    def border_color(self, color: Color) -> 'Container':


        result = questpdf.lib.questpdf__container__border_color__2a24bda0(self.target_pointer, color.hex)
        return Container(result)

    
    
    def border_alignment_outside(self) -> 'Container':


        result = questpdf.lib.questpdf__container__border_alignment_outside__ce5e63fa(self.target_pointer)
        return Container(result)

    
    
    def border_alignment_middle(self) -> 'Container':


        result = questpdf.lib.questpdf__container__border_alignment_middle__66a27445(self.target_pointer)
        return Container(result)

    
    
    def border_alignment_inside(self) -> 'Container':


        result = questpdf.lib.questpdf__container__border_alignment_inside__8cef56b1(self.target_pointer)
        return Container(result)

    
    
    def _svg_string(self, svg: str) -> 'SvgImageDescriptor':


        result = questpdf.lib.questpdf__container__svg__f547d46e(self.target_pointer, questpdf_ffi.new("char[]", svg.encode("utf-8")))
        return SvgImageDescriptor(result)

    
    
    def _svg_svg_image(self, image: SvgImage) -> 'SvgImageDescriptor':


        result = questpdf.lib.questpdf__container__svg__b1de06e3(self.target_pointer, image)
        return SvgImageDescriptor(result)

    
    
    def table(self, handler: Callable[['TableDescriptor'], Any]) -> None:

        @questpdf_ffi.callback("void(void*)")
        def _internal_handler_handler(handler_pointer):
            handler_object = TableDescriptor(handler_pointer)
            handler(handler_object)

        self._callbacks.append(_internal_handler_handler)


        result = questpdf.lib.questpdf__container__table__d49da987(self.target_pointer, _internal_handler_handler)
        

    
    
    def _text_action(self, content: Callable[['TextDescriptor'], Any]) -> None:

        @questpdf_ffi.callback("void(void*)")
        def _internal_content_handler(content_pointer):
            content_object = TextDescriptor(content_pointer)
            content(content_object)

        self._callbacks.append(_internal_content_handler)


        result = questpdf.lib.questpdf__container__text__357e362f(self.target_pointer, _internal_content_handler)
        

    
    
    def _text_string(self, text: str) -> 'TextBlockDescriptor':


        result = questpdf.lib.questpdf__container__text__3f6b5b07(self.target_pointer, questpdf_ffi.new("char[]", text.encode("utf-8")))
        return TextBlockDescriptor(result)

    
    
    def translate_x(self, value: float, unit: Unit = Unit.point) -> 'Container':


        result = questpdf.lib.questpdf__container__translate_x__351baebe(self.target_pointer, value, unit.value)
        return Container(result)

    
    
    def translate_y(self, value: float, unit: Unit = Unit.point) -> 'Container':


        result = questpdf.lib.questpdf__container__translate_y__d99602db(self.target_pointer, value, unit.value)
        return Container(result)


    def background_linear_gradient(self, angle: float, colors: list[Color]) -> 'Container':
        colors_hex = [color.hex for color in colors]
        colors_ffi = questpdf_ffi.new("uint32_t[]", colors_hex)
        result = questpdf.lib.questpdf__container__background_linear_gradient(self.target_pointer, angle, colors_ffi, len(colors_ffi))
        return Container(result)
    
    def border_linear_gradient(self, angle: float, colors: list[Color]) -> 'Container':
        colors_hex = [color.hex for color in colors]
        colors_ffi = questpdf_ffi.new("uint32_t[]", colors_hex)
        result = questpdf.lib.questpdf__container__border_linear_gradient(self.target_pointer, angle, colors_ffi, len(colors_ffi))
        return Container(result)

    def shadow(self, *, blur: float = 0, color: Color = Colors.black, offset_x: float = 0, offset_y: float = 0, spread: float = 0) -> 'Container':
        result = questpdf.lib.questpdf__container__shadow(self.target_pointer, blur, color.hex, offset_x, offset_y, spread)
        return Container(result)


    @overload
    def text(self, source: str) -> 'TextBlockDescriptor': ...

    @overload
    def text(self, source: Callable[['TextDescriptor'], list[Any]]) -> None: ...

    def text(self, source: Union[str, Callable[['TextDescriptor'], list[Any]]]) -> Optional['TextBlockDescriptor']:
        if isinstance(source, str):
            return self._text_string(source)
    
        elif callable(source):
            return self._text_action(source)
    
        else:
            raise TypeError(f"Expected str or Callable, got {type(source)}")
    
    def _svg_func(self, source: Callable[[DynamicSvgPayload], str]) -> None:
        @questpdf_ffi.callback("char*(DynamicSvgSourcePayload)")
        def native_callback(c_payload):
            payload = DynamicSvgPayload(c_payload)
            result_str = source(payload)
            return questpdf_ffi.new("char[]", result_str.encode('utf-8'))

        self._callbacks.append(native_callback)

        questpdf.lib.questpdf__container__svg_dynamic(self.target_pointer, native_callback)

    @overload
    def svg(self, source: str) -> 'SvgImageDescriptor': ...
    
    @overload
    def svg(self, source: SvgImage) -> 'SvgImageDescriptor': ...
    
    @overload
    def svg(self, source: Callable[['DynamicSvgPayload'], str]) -> None: ...
    
    def svg(self, source: Union[str, SvgImage, Callable[['DynamicSvgPayload'], str]]) -> Optional['SvgImageDescriptor']:
        if isinstance(source, str):
            return self._svg_string(source)
    
        elif isinstance(source, SvgImage):
            return self._svg_svg_image(source)
    
        elif callable(source):
            return self._svg_func(source)
    
        else:
            raise TypeError(f"Expected str, SvgImage, or Callable, got {type(source)}")



    def _image_bytes(self, data: bytes) -> 'ImageDescriptor':
        buffer = questpdf_ffi.new("Buffer*")
        buffer.data = questpdf_ffi.from_buffer("uint8_t[]", data)
        buffer.length = len(data)

        result = questpdf.lib.questpdf__container__image_bytes(self.target_pointer, buffer[0])
        return ImageDescriptor(result)

    def _image_func(self, source: Callable[[DynamicImagePayload], bytes]) -> 'ImageDescriptor':
        @questpdf_ffi.callback("Buffer(DynamicImageSourcePayload)")
        def native_callback(c_payload):
            payload = DynamicImagePayload(c_payload)
            result_bytes = source(payload)

            data_ptr = questpdf_ffi.new("uint8_t[]", result_bytes)

            buffer = questpdf_ffi.new("Buffer*")
            buffer.data = data_ptr
            buffer.length = len(result_bytes)
            return buffer[0]

        # Store callback reference to prevent garbage collection
        self._callbacks.append(native_callback)

        result = questpdf.lib.questpdf__container__image_dynamic(self.target_pointer, native_callback)
        return ImageDescriptor(result)

    

    @overload
    def image(self, source: str) -> 'ImageDescriptor': ...
    
    @overload
    def image(self, source: bytes) -> 'ImageDescriptor': ...
    
    @overload
    def image(self, source: Image) -> 'ImageDescriptor': ...
    
    @overload
    def image(self, source: Callable[['Size'], str]) -> 'DynamicImageDescriptor': ...
    
    def image(self, source: Union[str, bytes, 'Image', Callable[['DynamicImagePayload'], bytes]]) -> Union['ImageDescriptor', 'DynamicImageDescriptor']:
        if isinstance(source, str):
            return self._image_string(source)

        if isinstance(source, bytes):
            return self._image_bytes(source)

        elif isinstance(source, Image):
            return self._image_image(source)
    
        elif callable(source):
            return self._image_func(source)
    
        else:
            raise TypeError(f"Expected str, bytes, Image, or Callable, got {type(source)}")



QUESTPDF_CDEF += r"""
    

    
    void* questpdf__table_cell_container__column__384372f0(void* tableCellContainer, uint32_t value);
    
    void* questpdf__table_cell_container__column_span__629b3552(void* tableCellContainer, uint32_t value);
    
    void* questpdf__table_cell_container__row__7ddb9999(void* tableCellContainer, uint32_t value);
    
    void* questpdf__table_cell_container__row_span__e9016d30(void* tableCellContainer, uint32_t value);
    


"""



class TableCellContainer(Container):
    def __init__(self, target_pointer):
        self.target_pointer = target_pointer
        self._callbacks = []  # prevent GC of callbacks during native calls


    
    
    def column(self, value: int) -> 'TableCellContainer':


        result = questpdf.lib.questpdf__table_cell_container__column__384372f0(self.target_pointer, value)
        return TableCellContainer(result)

    
    
    def column_span(self, value: int) -> 'TableCellContainer':


        result = questpdf.lib.questpdf__table_cell_container__column_span__629b3552(self.target_pointer, value)
        return TableCellContainer(result)

    
    
    def row(self, value: int) -> 'TableCellContainer':


        result = questpdf.lib.questpdf__table_cell_container__row__7ddb9999(self.target_pointer, value)
        return TableCellContainer(result)

    
    
    def row_span(self, value: int) -> 'TableCellContainer':


        result = questpdf.lib.questpdf__table_cell_container__row_span__e9016d30(self.target_pointer, value)
        return TableCellContainer(result)










QUESTPDF_CDEF += r"""
    typedef void (*documentContainerDescriptorCallback)(void* document_container);
    typedef void (*pageDescriptorCallback)(void* page);

    int  questpdf_sum(int a, int b);

    void*  questpdf_document_create(documentContainerDescriptorCallback cb);
    Buffer questpdf_document_generate_pdf(void* document);
    void   questpdf_document_destroy(void* document);

    void   questpdf_document_container_add_page(void* document_container, pageDescriptorCallback cb);


    void   questpdf_page_set_margin(void* page, int margin);
    void*   questpdf_page_set_content(void* page);
    void*   questpdf_container_background(void* page, uint32_t color);

    void   questpdf_free_bytes(void* ptr);
"""




class DocumentContainer:
    def __init__(self, document_container_pointer: "questpdf_ffi.CData"):
        self.document_container_pointer = document_container_pointer
        # Keep Python refs to callbacks alive for as long as this container lives
        self._page_callbacks = []  # type: list["ffi.CData"]

    def page(self, configurator: Callable[[PageDescriptor], Any]) -> None:
        @questpdf_ffi.callback("void(void*)")
        def _internal_page_handler(page_pointer):
            page_object = PageDescriptor(page_pointer)
            configurator(page_object)

        # retain the callback to prevent it from being GC'ed
        self._page_callbacks.append(_internal_page_handler)
        questpdf.lib.questpdf_document_container_add_page(self.document_container_pointer, _internal_page_handler)



class Document:
    def __init__(self):
        self._document_ptr: Optional["questpdf_ffi.CData"] = None
        self._document_container_cb: Optional["questpdf_ffi.CData"] = None  # keep callback alive

    @classmethod
    def create(cls, configurator: Callable[[DocumentContainer], Any]) -> "Document":
        inst = cls()

        @questpdf_ffi.callback("void(void*)")
        def _internal_container_handler(document_container_pointer):
            document_container = DocumentContainer(document_container_pointer)
            configurator(document_container)

        inst._document_container_cb = _internal_container_handler
        inst._document_ptr = questpdf.lib.questpdf_document_create(inst._document_container_cb)
        return inst

    def generate_pdf(self) -> bytes:
        if self._document_ptr is None:
            raise RuntimeError("Document not created. Call create() first.")

        buf = questpdf.lib.questpdf_document_generate_pdf(self._document_ptr)  # returns Buffer by value
        try:
            # safer: copy to Python bytes, then free the native memory
            return questpdf_ffi.buffer(buf.data, buf.length)[:]
            # or: ffi.unpack(buf.data, buf.length)
        finally:
            questpdf.lib.questpdf_free_bytes(buf.data)

    def save_to_file(self, filename: str) -> "Document":
        pdf_bytes = self.generate_pdf()
        with open(filename, "wb") as f:
            f.write(pdf_bytes)
        return self

    def destroy(self) -> None:
        if self._document_ptr is not None:
            questpdf.lib.questpdf_document_destroy(self._document_ptr)
            self._document_ptr = None
            self._document_container_cb = None  # allow GC

    def __enter__(self):
        return self

    def __del__(self):
        # Make destruction idempotent and safe at interpreter shutdown
        try:
            self.destroy()
        except Exception:
            pass
        return False





questpdf_ffi.cdef(QUESTPDF_CDEF)

lib_path = os.path.abspath("./QuestPDF.Interop.dylib")
questpdf.lib = questpdf_ffi.dlopen(lib_path)

