// AUTO-GENERATED on 02/23/2026 17:01:08
// QuestPDF Kotlin Bindings using JNA (Java Native Access)

@file:Suppress("unused", "MemberVisibilityCanBePrivate", "FunctionName")

package com.questpdf.interop

import com.sun.jna.*
import java.io.File
import java.io.FileOutputStream
import java.util.Collections

var jnaOptions = Collections.singletonMap(Library.OPTION_STRING_ENCODING, "UTF-16LE");

private class QuestLibrary {
    companion object {
        private val initLock = Any()

        private fun getLibraryPath(): String {
            val os = System.getProperty("os.name").lowercase()
            val libName = when {
                "win" in os -> "QuestPDF.Interop.dll"
                "mac" in os -> "QuestPDF.Interop.dylib"
                else -> "libQuestPDF.Interop.so"
            }

            val searchPaths = listOf(
                libName,
                "./$libName",
                "../$libName",
                "${System.getProperty("user.dir")}/$libName"
            )

            for (path in searchPaths) {
                val file = java.io.File(path)
                if (file.exists()) {
                    return file.absolutePath
                }
            }

            return libName
        }

        fun <T : Library> getLibrary(interfaceClass : Class<T>): T {
            synchronized(initLock) {
                return Native.load(getLibraryPath(), interfaceClass, jnaOptions)
            }
        }
    }
}
        
        

// ============================================================================
// Native Buffer Structure
// ============================================================================

/**
 * Native buffer structure for receiving byte arrays from native code.
 */
@Structure.FieldOrder("data", "length")
open class NativeBuffer : Structure {
    @JvmField var data: Pointer? = null
    @JvmField var length: NativeLong = NativeLong(0)

    constructor() : super()
    constructor(p: Pointer) : super(p) {
        read()
    }

    class ByValue : NativeBuffer(), Structure.ByValue
}

// ============================================================================
// Callback Interfaces
// ============================================================================

/**
 * Callback interface for pointer-based callbacks.
 * JNA requires a single method interface extending Callback.
 */
interface PointerCallback : Callback {
    fun callback(ptr: Pointer)
}

// ============================================================================
// Native Library Interface
// ============================================================================

/**
 * JNA interface for the QuestPDF native library.
 */
interface QuestPDFNative : Library {
    // Test function
    fun questpdf_sum(a: Int, b: Int): Int

    // Document lifecycle
    fun questpdf_document_create(cb: PointerCallback): Pointer
    fun questpdf_document_generate_pdf(document: Pointer): NativeBuffer.ByValue
    fun questpdf_document_destroy(document: Pointer)

    // Document container
    fun questpdf_document_container_add_page(container: Pointer, cb: PointerCallback)

    // Page descriptor
    fun questpdf_page_set_margin(page: Pointer, margin: Int)
    fun questpdf_page_set_content(page: Pointer): Pointer

    // Container
    fun questpdf_container_background(container: Pointer, color: Int): Pointer

    // Memory management
    fun questpdf_free_bytes(ptr: Pointer)
}

// ============================================================================
// Library Loading
// ============================================================================

/**
 * QuestPDF library manager for loading and accessing the native library.
 */
object QuestPDF {
    @Volatile
    private var lib: QuestPDFNative? = null
    private val lock = Any()

    /**
     * Gets the native library instance.
     * @throws IllegalStateException if the library is not initialized.
     */
    fun getLib(): QuestPDFNative {
        return lib ?: throw IllegalStateException("QuestPDF library not initialized. Call initialize() first.")
    }

    /**
     * Initializes the QuestPDF library with automatic platform detection.
     */
    fun initialize() {
        initialize(null)
    }

    /**
     * Initializes the QuestPDF library with a specific library path.
     * @param libraryPath Path to the native library, or null for auto-detection.
     */
    fun initialize(libraryPath: String?) {
        if (lib != null) return

        synchronized(lock) {
            if (lib != null) return

            val libPath = libraryPath ?: getDefaultLibraryPath()
            lib = Native.load(libPath, QuestPDFNative::class.java, jnaOptions)
        }
    }

    /**
     * Tests if the native library is working correctly.
     * @return true if the test passes.
     */
    fun test(): Boolean {
        return try {
            val result = getLib().questpdf_sum(2, 3)
            result == 5
        } catch (e: Exception) {
            false
        }
    }

    private fun getDefaultLibraryPath(): String {
        val os = System.getProperty("os.name").lowercase()
        val libName = when {
            "win" in os -> "QuestPDF.Interop.dll"
            "mac" in os -> "QuestPDF.Interop.dylib"
            else -> "libQuestPDF.Interop.so"
        }

        // Try multiple locations
        val searchPaths = listOf(
            libName,
            "./$libName",
            "../$libName",
            "${System.getProperty("user.dir")}/$libName"
        )

        for (path in searchPaths) {
            val file = File(path)
            if (file.exists()) {
                return file.absolutePath
            }
        }

        // Fall back to library name only, let JNA resolve it
        return libName
    }
}

/**
 * Represents a document container for adding pages.
 */
class DocumentContainer internal constructor(val pointer: Pointer) {
    // Keep strong references to callbacks to prevent GC
    private val callbacks = mutableListOf<Callback>()

    /**
     * Adds a page to the document.
     */
    fun page(configurator: (PageDescriptor) -> Unit) {
        val callback = object : PointerCallback {
            override fun callback(ptr: Pointer) {
                val page = PageDescriptor(ptr)
                configurator(page)
            }
        }

        // Keep callback reference to prevent GC
        callbacks.add(callback)
        QuestPDF.getLib().questpdf_document_container_add_page(pointer, callback)
    }
}

/**
 * Represents a QuestPDF document.
 */
class Document private constructor() {
    private var ptr: Pointer? = null
    // Keep strong references to callbacks to prevent GC
    private val callbacks = mutableListOf<Callback>()

    companion object {
        /**
         * Creates a new document with the given configuration.
         */
        fun create(configurator: (DocumentContainer) -> Unit): Document {
            val doc = Document()

            val containerCallback = object : PointerCallback {
                override fun callback(ptr: Pointer) {
                    val container = DocumentContainer(ptr)
                    configurator(container)
                }
            }

            // Keep callback reference to prevent GC
            doc.callbacks.add(containerCallback)
            doc.ptr = QuestPDF.getLib().questpdf_document_create(containerCallback)
            return doc
        }
    }

    /**
     * Generates the PDF as a byte array.
     */
    fun generatePdf(): ByteArray {
        val currentPtr = ptr ?: throw IllegalStateException("Document not created. Use Document.create() first.")

        val buffer = QuestPDF.getLib().questpdf_document_generate_pdf(currentPtr)
        return try {
            val length = buffer.length.toInt()
            buffer.data!!.getByteArray(0, length)
        } finally {
            QuestPDF.getLib().questpdf_free_bytes(buffer.data!!)
        }
    }

    /**
     * Saves the PDF to a file.
     */
    fun saveToFile(filename: String): Document {
        val pdfBytes = generatePdf()
        FileOutputStream(filename).use { fos ->
            fos.write(pdfBytes)
        }
        return this
    }

    /**
     * Destroys the document and frees native resources.
     */
    fun destroy() {
        ptr?.let {
            QuestPDF.getLib().questpdf_document_destroy(it)
            ptr = null
            callbacks.clear()
        }
    }
}

// ============================================================================
// Generated Code
// ============================================================================


/**
 * Represents a color with ARGB components.
 */
@JvmInline
value class Color(val hex: UInt) {

}

/**
 * Predefined color constants.
 */
object Colors {

    val Black = Color(0xFF000000u)

    val White = Color(0xFFFFFFFFu)

    val Transparent = Color(0x00000000u)



    object Red {

        val Lighten5 = Color(0xFFFFEBEEu)

        val Lighten4 = Color(0xFFFFCDD2u)

        val Lighten3 = Color(0xFFEF9A9Au)

        val Lighten2 = Color(0xFFE57373u)

        val Lighten1 = Color(0xFFEF5350u)

        val Medium = Color(0xFFF44336u)

        val Darken1 = Color(0xFFE53935u)

        val Darken2 = Color(0xFFD32F2Fu)

        val Darken3 = Color(0xFFC62828u)

        val Darken4 = Color(0xFFB71C1Cu)

        val Accent1 = Color(0xFFFF8A80u)

        val Accent2 = Color(0xFFFF5252u)

        val Accent3 = Color(0xFFFF1744u)

        val Accent4 = Color(0xFFD50000u)

    }


    object Pink {

        val Lighten5 = Color(0xFFFCE4ECu)

        val Lighten4 = Color(0xFFF8BBD0u)

        val Lighten3 = Color(0xFFF48FB1u)

        val Lighten2 = Color(0xFFF06292u)

        val Lighten1 = Color(0xFFEC407Au)

        val Medium = Color(0xFFE91E63u)

        val Darken1 = Color(0xFFD81B60u)

        val Darken2 = Color(0xFFC2185Bu)

        val Darken3 = Color(0xFFAD1457u)

        val Darken4 = Color(0xFF880E4Fu)

        val Accent1 = Color(0xFFFF80ABu)

        val Accent2 = Color(0xFFFF4081u)

        val Accent3 = Color(0xFFF50057u)

        val Accent4 = Color(0xFFC51162u)

    }


    object Purple {

        val Lighten5 = Color(0xFFF3E5F5u)

        val Lighten4 = Color(0xFFE1BEE7u)

        val Lighten3 = Color(0xFFCE93D8u)

        val Lighten2 = Color(0xFFBA68C8u)

        val Lighten1 = Color(0xFFAB47BCu)

        val Medium = Color(0xFF9C27B0u)

        val Darken1 = Color(0xFF8E24AAu)

        val Darken2 = Color(0xFF7B1FA2u)

        val Darken3 = Color(0xFF6A1B9Au)

        val Darken4 = Color(0xFF4A148Cu)

        val Accent1 = Color(0xFFEA80FCu)

        val Accent2 = Color(0xFFE040FBu)

        val Accent3 = Color(0xFFD500F9u)

        val Accent4 = Color(0xFFAA00FFu)

    }


    object DeepPurple {

        val Lighten5 = Color(0xFFEDE7F6u)

        val Lighten4 = Color(0xFFD1C4E9u)

        val Lighten3 = Color(0xFFB39DDBu)

        val Lighten2 = Color(0xFF9575CDu)

        val Lighten1 = Color(0xFF7E57C2u)

        val Medium = Color(0xFF673AB7u)

        val Darken1 = Color(0xFF5E35B1u)

        val Darken2 = Color(0xFF512DA8u)

        val Darken3 = Color(0xFF4527A0u)

        val Darken4 = Color(0xFF311B92u)

        val Accent1 = Color(0xFFB388FFu)

        val Accent2 = Color(0xFF7C4DFFu)

        val Accent3 = Color(0xFF651FFFu)

        val Accent4 = Color(0xFF6200EAu)

    }


    object Indigo {

        val Lighten5 = Color(0xFFE8EAF6u)

        val Lighten4 = Color(0xFFC5CAE9u)

        val Lighten3 = Color(0xFF9FA8DAu)

        val Lighten2 = Color(0xFF7986CBu)

        val Lighten1 = Color(0xFF5C6BC0u)

        val Medium = Color(0xFF3F51B5u)

        val Darken1 = Color(0xFF3949ABu)

        val Darken2 = Color(0xFF303F9Fu)

        val Darken3 = Color(0xFF283593u)

        val Darken4 = Color(0xFF1A237Eu)

        val Accent1 = Color(0xFF8C9EFFu)

        val Accent2 = Color(0xFF536DFEu)

        val Accent3 = Color(0xFF3D5AFEu)

        val Accent4 = Color(0xFF304FFEu)

    }


    object Blue {

        val Lighten5 = Color(0xFFE3F2FDu)

        val Lighten4 = Color(0xFFBBDEFBu)

        val Lighten3 = Color(0xFF90CAF9u)

        val Lighten2 = Color(0xFF64B5F6u)

        val Lighten1 = Color(0xFF42A5F5u)

        val Medium = Color(0xFF2196F3u)

        val Darken1 = Color(0xFF1E88E5u)

        val Darken2 = Color(0xFF1976D2u)

        val Darken3 = Color(0xFF1565C0u)

        val Darken4 = Color(0xFF0D47A1u)

        val Accent1 = Color(0xFF82B1FFu)

        val Accent2 = Color(0xFF448AFFu)

        val Accent3 = Color(0xFF2979FFu)

        val Accent4 = Color(0xFF2962FFu)

    }


    object LightBlue {

        val Lighten5 = Color(0xFFE1F5FEu)

        val Lighten4 = Color(0xFFB3E5FCu)

        val Lighten3 = Color(0xFF81D4FAu)

        val Lighten2 = Color(0xFF4FC3F7u)

        val Lighten1 = Color(0xFF29B6F6u)

        val Medium = Color(0xFF03A9F4u)

        val Darken1 = Color(0xFF039BE5u)

        val Darken2 = Color(0xFF0288D1u)

        val Darken3 = Color(0xFF0277BDu)

        val Darken4 = Color(0xFF01579Bu)

        val Accent1 = Color(0xFF80D8FFu)

        val Accent2 = Color(0xFF40C4FFu)

        val Accent3 = Color(0xFF00B0FFu)

        val Accent4 = Color(0xFF0091EAu)

    }


    object Cyan {

        val Lighten5 = Color(0xFFE0F7FAu)

        val Lighten4 = Color(0xFFB2EBF2u)

        val Lighten3 = Color(0xFF80DEEAu)

        val Lighten2 = Color(0xFF4DD0E1u)

        val Lighten1 = Color(0xFF26C6DAu)

        val Medium = Color(0xFF00BCD4u)

        val Darken1 = Color(0xFF00ACC1u)

        val Darken2 = Color(0xFF0097A7u)

        val Darken3 = Color(0xFF00838Fu)

        val Darken4 = Color(0xFF006064u)

        val Accent1 = Color(0xFF84FFFFu)

        val Accent2 = Color(0xFF18FFFFu)

        val Accent3 = Color(0xFF00E5FFu)

        val Accent4 = Color(0xFF00B8D4u)

    }


    object Teal {

        val Lighten5 = Color(0xFFE0F2F1u)

        val Lighten4 = Color(0xFFB2DFDBu)

        val Lighten3 = Color(0xFF80CBC4u)

        val Lighten2 = Color(0xFF4DB6ACu)

        val Lighten1 = Color(0xFF26A69Au)

        val Medium = Color(0xFF009688u)

        val Darken1 = Color(0xFF00897Bu)

        val Darken2 = Color(0xFF00796Bu)

        val Darken3 = Color(0xFF00695Cu)

        val Darken4 = Color(0xFF004D40u)

        val Accent1 = Color(0xFFA7FFEBu)

        val Accent2 = Color(0xFF64FFDAu)

        val Accent3 = Color(0xFF1DE9B6u)

        val Accent4 = Color(0xFF00BFA5u)

    }


    object Green {

        val Lighten5 = Color(0xFFE8F5E9u)

        val Lighten4 = Color(0xFFC8E6C9u)

        val Lighten3 = Color(0xFFA5D6A7u)

        val Lighten2 = Color(0xFF81C784u)

        val Lighten1 = Color(0xFF66BB6Au)

        val Medium = Color(0xFF4CAF50u)

        val Darken1 = Color(0xFF43A047u)

        val Darken2 = Color(0xFF388E3Cu)

        val Darken3 = Color(0xFF2E7D32u)

        val Darken4 = Color(0xFF1B5E20u)

        val Accent1 = Color(0xFFB9F6CAu)

        val Accent2 = Color(0xFF69F0AEu)

        val Accent3 = Color(0xFF00E676u)

        val Accent4 = Color(0xFF00C853u)

    }


    object LightGreen {

        val Lighten5 = Color(0xFFF1F8E9u)

        val Lighten4 = Color(0xFFDCEDC8u)

        val Lighten3 = Color(0xFFC5E1A5u)

        val Lighten2 = Color(0xFFAED581u)

        val Lighten1 = Color(0xFF9CCC65u)

        val Medium = Color(0xFF8BC34Au)

        val Darken1 = Color(0xFF7CB342u)

        val Darken2 = Color(0xFF689F38u)

        val Darken3 = Color(0xFF558B2Fu)

        val Darken4 = Color(0xFF33691Eu)

        val Accent1 = Color(0xFFCCFF90u)

        val Accent2 = Color(0xFFB2FF59u)

        val Accent3 = Color(0xFF76FF03u)

        val Accent4 = Color(0xFF64DD17u)

    }


    object Lime {

        val Lighten5 = Color(0xFFF9FBE7u)

        val Lighten4 = Color(0xFFF0F4C3u)

        val Lighten3 = Color(0xFFE6EE9Cu)

        val Lighten2 = Color(0xFFDCE775u)

        val Lighten1 = Color(0xFFD4E157u)

        val Medium = Color(0xFFCDDC39u)

        val Darken1 = Color(0xFFC0CA33u)

        val Darken2 = Color(0xFFAFB42Bu)

        val Darken3 = Color(0xFF9E9D24u)

        val Darken4 = Color(0xFF827717u)

        val Accent1 = Color(0xFFF4FF81u)

        val Accent2 = Color(0xFFEEFF41u)

        val Accent3 = Color(0xFFC6FF00u)

        val Accent4 = Color(0xFFAEEA00u)

    }


    object Yellow {

        val Lighten5 = Color(0xFFFFFDE7u)

        val Lighten4 = Color(0xFFFFF9C4u)

        val Lighten3 = Color(0xFFFFF59Du)

        val Lighten2 = Color(0xFFFFF176u)

        val Lighten1 = Color(0xFFFFEE58u)

        val Medium = Color(0xFFFFEB3Bu)

        val Darken1 = Color(0xFFFDD835u)

        val Darken2 = Color(0xFFFBC02Du)

        val Darken3 = Color(0xFFF9A825u)

        val Darken4 = Color(0xFFF57F17u)

        val Accent1 = Color(0xFFFFFF8Du)

        val Accent2 = Color(0xFFFFFF00u)

        val Accent3 = Color(0xFFFFEA00u)

        val Accent4 = Color(0xFFFFD600u)

    }


    object Amber {

        val Lighten5 = Color(0xFFFFF8E1u)

        val Lighten4 = Color(0xFFFFECB3u)

        val Lighten3 = Color(0xFFFFE082u)

        val Lighten2 = Color(0xFFFFD54Fu)

        val Lighten1 = Color(0xFFFFCA28u)

        val Medium = Color(0xFFFFC107u)

        val Darken1 = Color(0xFFFFB300u)

        val Darken2 = Color(0xFFFFA000u)

        val Darken3 = Color(0xFFFF8F00u)

        val Darken4 = Color(0xFFFF6F00u)

        val Accent1 = Color(0xFFFFE57Fu)

        val Accent2 = Color(0xFFFFD740u)

        val Accent3 = Color(0xFFFFC400u)

        val Accent4 = Color(0xFFFFAB00u)

    }


    object Orange {

        val Lighten5 = Color(0xFFFFF3E0u)

        val Lighten4 = Color(0xFFFFE0B2u)

        val Lighten3 = Color(0xFFFFCC80u)

        val Lighten2 = Color(0xFFFFB74Du)

        val Lighten1 = Color(0xFFFFA726u)

        val Medium = Color(0xFFFF9800u)

        val Darken1 = Color(0xFFFB8C00u)

        val Darken2 = Color(0xFFF57C00u)

        val Darken3 = Color(0xFFEF6C00u)

        val Darken4 = Color(0xFFE65100u)

        val Accent1 = Color(0xFFFFD180u)

        val Accent2 = Color(0xFFFFAB40u)

        val Accent3 = Color(0xFFFF9100u)

        val Accent4 = Color(0xFFFF6D00u)

    }


    object DeepOrange {

        val Lighten5 = Color(0xFFFBE9E7u)

        val Lighten4 = Color(0xFFFFCCBCu)

        val Lighten3 = Color(0xFFFFAB91u)

        val Lighten2 = Color(0xFFFF8A65u)

        val Lighten1 = Color(0xFFFF7043u)

        val Medium = Color(0xFFFF5722u)

        val Darken1 = Color(0xFFF4511Eu)

        val Darken2 = Color(0xFFE64A19u)

        val Darken3 = Color(0xFFD84315u)

        val Darken4 = Color(0xFFBF360Cu)

        val Accent1 = Color(0xFFFF9E80u)

        val Accent2 = Color(0xFFFF6E40u)

        val Accent3 = Color(0xFFFF3D00u)

        val Accent4 = Color(0xFFDD2C00u)

    }


    object Brown {

        val Lighten5 = Color(0xFFEFEBE9u)

        val Lighten4 = Color(0xFFD7CCC8u)

        val Lighten3 = Color(0xFFBCAAA4u)

        val Lighten2 = Color(0xFFA1887Fu)

        val Lighten1 = Color(0xFF8D6E63u)

        val Medium = Color(0xFF795548u)

        val Darken1 = Color(0xFF6D4C41u)

        val Darken2 = Color(0xFF5D4037u)

        val Darken3 = Color(0xFF4E342Eu)

        val Darken4 = Color(0xFF3E2723u)

    }


    object Grey {

        val Lighten5 = Color(0xFFFAFAFAu)

        val Lighten4 = Color(0xFFF5F5F5u)

        val Lighten3 = Color(0xFFEEEEEEu)

        val Lighten2 = Color(0xFFE0E0E0u)

        val Lighten1 = Color(0xFFBDBDBDu)

        val Medium = Color(0xFF9E9E9Eu)

        val Darken1 = Color(0xFF757575u)

        val Darken2 = Color(0xFF616161u)

        val Darken3 = Color(0xFF424242u)

        val Darken4 = Color(0xFF212121u)

    }


    object BlueGrey {

        val Lighten5 = Color(0xFFECEFF1u)

        val Lighten4 = Color(0xFFCFD8DCu)

        val Lighten3 = Color(0xFFB0BEC5u)

        val Lighten2 = Color(0xFF90A4AEu)

        val Lighten1 = Color(0xFF78909Cu)

        val Medium = Color(0xFF607D8Bu)

        val Darken1 = Color(0xFF546E7Au)

        val Darken2 = Color(0xFF455A64u)

        val Darken3 = Color(0xFF37474Fu)

        val Darken4 = Color(0xFF263238u)

    }


}



enum class AspectRatioOption(val value: Int) {
    FitWidth(0),
    FitHeight(1),
    FitArea(2);
}


enum class ContentDirection(val value: Int) {
    LeftToRight(0),
    RightToLeft(1);
}


enum class FontWeight(val value: Int) {
    Thin(100),
    ExtraLight(200),
    Light(300),
    Normal(400),
    Medium(500),
    SemiBold(600),
    Bold(700),
    ExtraBold(800),
    Black(900),
    ExtraBlack(1000);
}


enum class ImageCompressionQuality(val value: Int) {
    Best(0),
    VeryHigh(1),
    High(2),
    Medium(3),
    Low(4),
    VeryLow(5);
}


enum class ImageFormat(val value: Int) {
    Jpeg(0),
    Png(1),
    Webp(2);
}


enum class ImageScaling(val value: Int) {
    FitWidth(0),
    FitHeight(1),
    FitArea(2),
    Resize(3);
}


enum class LicenseType(val value: Int) {
    Community(0),
    Professional(1),
    Enterprise(2);
}


enum class TextHorizontalAlignment(val value: Int) {
    Left(0),
    Center(1),
    Right(2),
    Justify(3),
    Start(4),
    End(5);
}


enum class TextInjectedElementAlignment(val value: Int) {
    AboveBaseline(0),
    BelowBaseline(1),
    Top(2),
    Bottom(3),
    Middle(4);
}


enum class LengthUnit(val value: Int) {
    Point(0),
    Meter(1),
    Centimetre(2),
    Millimetre(3),
    Feet(4),
    Inch(5),
    Mil(6);
}




interface SettingsLibrary : Library {
    companion object {
        val INSTANCE: SettingsLibrary = QuestLibrary.getLibrary(SettingsLibrary::class.java)
    }

    fun questpdf__settings__license(value: Int): Unit;
    fun questpdf__settings__enable_debugging(value: Boolean): Unit;
    fun questpdf__settings__enable_caching(value: Boolean): Unit;
    fun questpdf__settings__check_if_all_text_glyphs_are_available(value: Boolean): Unit;
    fun questpdf__settings__use_environment_fonts(value: Boolean): Unit;
    fun questpdf__settings__temporary_storage_path(value: String): Unit;
    fun questpdf__settings__font_discovery_paths(delimitedList: String): Unit;
}

class Settings {
    companion object {
        var license : LicenseType? = null
            set(value) {
                if (value == null) return;
                SettingsLibrary.INSTANCE.questpdf__settings__license(value.value)
            }

        var enableDebugging = false
            set(value) {
                SettingsLibrary.INSTANCE.questpdf__settings__enable_debugging(value)
            }

        var enableCaching = false
            set(value) {
                SettingsLibrary.INSTANCE.questpdf__settings__enable_caching(value)
            }

        var checkIfAllTextGlyphsAreAvailable = false
            set(value) {
                SettingsLibrary.INSTANCE.questpdf__settings__check_if_all_text_glyphs_are_available(value)
            }

        var useEnvironmentFonts = false
            set(value) {
                SettingsLibrary.INSTANCE.questpdf__settings__use_environment_fonts(value)
            }

        var temporaryStoragePath = ""
            set(value) {
                SettingsLibrary.INSTANCE.questpdf__settings__temporary_storage_path(value)
            }

        var fontDiscoveryPaths : Array<String> = arrayOf()
            set(value) {
                SettingsLibrary.INSTANCE.questpdf__settings__font_discovery_paths(value.joinToString("__questpdf__"))
            }
    }
}





// ============================================================================
// FontFeatures - Native Function Interface
// ============================================================================



/**
 * JNA interface for FontFeatures native methods.
 */
private interface FontFeaturesNative : Library {

    fun questpdf__font_features__character_variant__3a2f9f96(value: Int): String

    fun questpdf__font_features__stylistic_set__19e9e157(value: Int): String



}

// ============================================================================
// FontFeatures Callback Interfaces
// ============================================================================


// ============================================================================
// FontFeatures Class
// ============================================================================

/**
 * FontFeatures wrapper for the native QuestPDF FontFeatures.
 */
class FontFeatures internal constructor(val pointer: Pointer) {
    private val callbacks = mutableListOf<Callback>()

    companion object {
        private var nativeLib: FontFeaturesNative? = QuestLibrary.getLibrary(FontFeaturesNative::class.java)
        private val initLock = Any()

        private fun lib(): FontFeaturesNative {
            return nativeLib!!
        }

    
         

@JvmStatic
fun characterVariant(value: Int): String {


    return lib().questpdf__font_features__character_variant__3a2f9f96(value)

}
 
    
         

@JvmStatic
fun stylisticSet(value: Int): String {


    return lib().questpdf__font_features__stylistic_set__19e9e157(value)

}
 
    
    }
    






    
}




// ============================================================================
// Placeholders - Native Function Interface
// ============================================================================



/**
 * JNA interface for Placeholders native methods.
 */
private interface PlaceholdersNative : Library {

    fun questpdf__placeholders__lorem_ipsum__39e2dfef(): String

    fun questpdf__placeholders__label__06cbd381(): String

    fun questpdf__placeholders__sentence__d64f6e68(): String

    fun questpdf__placeholders__question__21de659f(): String

    fun questpdf__placeholders__paragraph__56235510(): String

    fun questpdf__placeholders__paragraphs__addb8834(): String

    fun questpdf__placeholders__email__e6405590(): String

    fun questpdf__placeholders__name__e536835d(): String

    fun questpdf__placeholders__phone_number__25314f3f(): String

    fun questpdf__placeholders__webpage_url__6e903669(): String

    fun questpdf__placeholders__price__0ae34c02(): String

    fun questpdf__placeholders__time__af3e33a8(): String

    fun questpdf__placeholders__short_date__f6b7ade4(): String

    fun questpdf__placeholders__long_date__736f3796(): String

    fun questpdf__placeholders__date_time__b29d44f5(): String

    fun questpdf__placeholders__integer__e8bfa000(): String

    fun questpdf__placeholders__decimal__4569a5a1(): String

    fun questpdf__placeholders__percent__3fa245e1(): String

    fun questpdf__placeholders__background_color__5395a374(): Int

    fun questpdf__placeholders__color__40d13de0(): Int

    fun questpdf__placeholders__image__a8827df6(width: Int, height: Int): Pointer

    fun questpdf__placeholders__image__2891dd9e(size: Pointer): Pointer



}

// ============================================================================
// Placeholders Callback Interfaces
// ============================================================================


// ============================================================================
// Placeholders Class
// ============================================================================

/**
 * Placeholders wrapper for the native QuestPDF Placeholders.
 */
class Placeholders internal constructor(val pointer: Pointer) {
    private val callbacks = mutableListOf<Callback>()

    companion object {
        private var nativeLib: PlaceholdersNative? = QuestLibrary.getLibrary(PlaceholdersNative::class.java)
        private val initLock = Any()

        private fun lib(): PlaceholdersNative {
            return nativeLib!!
        }

    
         

@JvmStatic
fun loremIpsum(): String {


    return lib().questpdf__placeholders__lorem_ipsum__39e2dfef()

}
 
    
         

@JvmStatic
fun label(): String {


    return lib().questpdf__placeholders__label__06cbd381()

}
 
    
         

@JvmStatic
fun sentence(): String {


    return lib().questpdf__placeholders__sentence__d64f6e68()

}
 
    
         

@JvmStatic
fun question(): String {


    return lib().questpdf__placeholders__question__21de659f()

}
 
    
         

@JvmStatic
fun paragraph(): String {


    return lib().questpdf__placeholders__paragraph__56235510()

}
 
    
         

@JvmStatic
fun paragraphs(): String {


    return lib().questpdf__placeholders__paragraphs__addb8834()

}
 
    
         

@JvmStatic
fun email(): String {


    return lib().questpdf__placeholders__email__e6405590()

}
 
    
         

@JvmStatic
fun name(): String {


    return lib().questpdf__placeholders__name__e536835d()

}
 
    
         

@JvmStatic
fun phoneNumber(): String {


    return lib().questpdf__placeholders__phone_number__25314f3f()

}
 
    
         

@JvmStatic
fun webpageUrl(): String {


    return lib().questpdf__placeholders__webpage_url__6e903669()

}
 
    
         

@JvmStatic
fun price(): String {


    return lib().questpdf__placeholders__price__0ae34c02()

}
 
    
         

@JvmStatic
fun time(): String {


    return lib().questpdf__placeholders__time__af3e33a8()

}
 
    
         

@JvmStatic
fun shortDate(): String {


    return lib().questpdf__placeholders__short_date__f6b7ade4()

}
 
    
         

@JvmStatic
fun longDate(): String {


    return lib().questpdf__placeholders__long_date__736f3796()

}
 
    
         

@JvmStatic
fun dateTime(): String {


    return lib().questpdf__placeholders__date_time__b29d44f5()

}
 
    
         

@JvmStatic
fun integer(): String {


    return lib().questpdf__placeholders__integer__e8bfa000()

}
 
    
         

@JvmStatic
fun decimal(): String {


    return lib().questpdf__placeholders__decimal__4569a5a1()

}
 
    
         

@JvmStatic
fun percent(): String {


    return lib().questpdf__placeholders__percent__3fa245e1()

}
 
    
         

@JvmStatic
fun backgroundColor(): Color {


    return lib().questpdf__placeholders__background_color__5395a374()

}
 
    
         

@JvmStatic
fun color(): Color {


    return lib().questpdf__placeholders__color__40d13de0()

}
 
    
         

@JvmStatic
fun image(width: Int, height: Int): Any {


    return lib().questpdf__placeholders__image__a8827df6(width, height)

}
 
    
         

@JvmStatic
fun image(size: Any): Any {


    return lib().questpdf__placeholders__image__2891dd9e(size)

}
 
    
    }
    














































    
}




// ============================================================================
// FontManager - Native Function Interface
// ============================================================================



/**
 * JNA interface for FontManager native methods.
 */
private interface FontManagerNative : Library {



}

// ============================================================================
// FontManager Callback Interfaces
// ============================================================================


// ============================================================================
// FontManager Class
// ============================================================================

/**
 * FontManager wrapper for the native QuestPDF FontManager.
 */
class FontManager internal constructor(val pointer: Pointer) {
    private val callbacks = mutableListOf<Callback>()

    companion object {
        private var nativeLib: FontManagerNative? = QuestLibrary.getLibrary(FontManagerNative::class.java)
        private val initLock = Any()

        private fun lib(): FontManagerNative {
            return nativeLib!!
        }

    
    }
    


    
}




// ============================================================================
// Image - Native Function Interface
// ============================================================================



/**
 * JNA interface for Image native methods.
 */
private interface ImageNative : Library {

    fun questpdf__image__from_file__d83c4447(filePath: String): Pointer



}

// ============================================================================
// Image Callback Interfaces
// ============================================================================


// ============================================================================
// Image Class
// ============================================================================

/**
 * Image wrapper for the native QuestPDF Image.
 */
class Image internal constructor(val pointer: Pointer) {
    private val callbacks = mutableListOf<Callback>()

    companion object {
        private var nativeLib: ImageNative? = QuestLibrary.getLibrary(ImageNative::class.java)
        private val initLock = Any()

        private fun lib(): ImageNative {
            return nativeLib!!
        }

    
         

@JvmStatic
fun fromFile(filePath: String): Image {


    val result = lib().questpdf__image__from_file__d83c4447(filePath)
    return Image(result)

}
 
    
    }
    




    
}




// ============================================================================
// SvgImage - Native Function Interface
// ============================================================================



/**
 * JNA interface for SvgImage native methods.
 */
private interface SvgImageNative : Library {

    fun questpdf__svg_image__from_file__79e2c64d(filePath: String): Pointer

    fun questpdf__svg_image__from_text__82b4cf0f(svg: String): Pointer



}

// ============================================================================
// SvgImage Callback Interfaces
// ============================================================================


// ============================================================================
// SvgImage Class
// ============================================================================

/**
 * SvgImage wrapper for the native QuestPDF SvgImage.
 */
class SvgImage internal constructor(val pointer: Pointer) {
    private val callbacks = mutableListOf<Callback>()

    companion object {
        private var nativeLib: SvgImageNative? = QuestLibrary.getLibrary(SvgImageNative::class.java)
        private val initLock = Any()

        private fun lib(): SvgImageNative {
            return nativeLib!!
        }

    
         

@JvmStatic
fun fromFile(filePath: String): SvgImage {


    val result = lib().questpdf__svg_image__from_file__79e2c64d(filePath)
    return SvgImage(result)

}
 
    
         

@JvmStatic
fun fromText(svg: String): SvgImage {


    val result = lib().questpdf__svg_image__from_text__82b4cf0f(svg)
    return SvgImage(result)

}
 
    
    }
    






    
}




// ============================================================================
// LineDescriptor - Native Function Interface
// ============================================================================



/**
 * JNA interface for LineDescriptor native methods.
 */
private interface LineDescriptorNative : Library {

    fun questpdf__line_descriptor__line_color__a86ca4e3(target: Pointer, color: Int): Pointer


fun questpdf__line_descriptor__line_dash_pattern(target: Pointer, values: FloatArray, valuesLength: Int, unit: Int): Pointer
fun questpdf__line_descriptor__line_gradient(target: Pointer, colors: IntArray, colorsLength: Int): Pointer
}

// ============================================================================
// LineDescriptor Callback Interfaces
// ============================================================================


// ============================================================================
// LineDescriptor Class
// ============================================================================

/**
 * LineDescriptor wrapper for the native QuestPDF LineDescriptor.
 */
class LineDescriptor internal constructor(val pointer: Pointer) {
    private val callbacks = mutableListOf<Callback>()

    companion object {
        private var nativeLib: LineDescriptorNative? = QuestLibrary.getLibrary(LineDescriptorNative::class.java)
        private val initLock = Any()

        private fun lib(): LineDescriptorNative {
            return nativeLib!!
        }

    
        
    
    }
    

 


fun lineColor(color: Color): LineDescriptor {


    val result = lib().questpdf__line_descriptor__line_color__a86ca4e3(pointer, color.hex.toInt())
    return LineDescriptor(result)

}
 


    fun lineDashPattern(pattern: Collection<Float>, unit: LengthUnit = LengthUnit.Point): LineDescriptor {
    val result = lib().questpdf__line_descriptor__line_dash_pattern(pointer, pattern.toFloatArray(), pattern.size, unit.value)
    return LineDescriptor(result)
}

fun lineGradient(colors: Collection<Color>): LineDescriptor {
    val colorsFfi = colors.map { it.hex.toInt() }.toIntArray()    
    val result = lib().questpdf__line_descriptor__line_gradient(pointer, colorsFfi, colorsFfi.size)
    return LineDescriptor(result)
}

}




// ============================================================================
// ColumnDescriptor - Native Function Interface
// ============================================================================



/**
 * JNA interface for ColumnDescriptor native methods.
 */
private interface ColumnDescriptorNative : Library {

    fun questpdf__column_descriptor__spacing__e47553e3(target: Pointer, value: Float, unit: Int): Unit

    fun questpdf__column_descriptor__item__2cf2ad89(target: Pointer): Pointer



}

// ============================================================================
// ColumnDescriptor Callback Interfaces
// ============================================================================


// ============================================================================
// ColumnDescriptor Class
// ============================================================================

/**
 * ColumnDescriptor wrapper for the native QuestPDF ColumnDescriptor.
 */
class ColumnDescriptor internal constructor(val pointer: Pointer) {
    private val callbacks = mutableListOf<Callback>()

    companion object {
        private var nativeLib: ColumnDescriptorNative? = QuestLibrary.getLibrary(ColumnDescriptorNative::class.java)
        private val initLock = Any()

        private fun lib(): ColumnDescriptorNative {
            return nativeLib!!
        }

    
        
    
        
    
    }
    

 


fun spacing(value: Float, unit: LengthUnit = LengthUnit.Point) {


    lib().questpdf__column_descriptor__spacing__e47553e3(pointer, value, unit.value)

}
 

 


fun item(): Container {


    val result = lib().questpdf__column_descriptor__item__2cf2ad89(pointer)
    return Container(result)

}
 


    
}




// ============================================================================
// DecorationDescriptor - Native Function Interface
// ============================================================================



/**
 * JNA interface for DecorationDescriptor native methods.
 */
private interface DecorationDescriptorNative : Library {

    fun questpdf__decoration_descriptor__before__1bfecdf8(target: Pointer): Pointer

    fun questpdf__decoration_descriptor__before__bf5ce29e(target: Pointer, handler: Callback): Unit

    fun questpdf__decoration_descriptor__content__9ec35667(target: Pointer): Pointer

    fun questpdf__decoration_descriptor__content__391a971a(target: Pointer, handler: Callback): Unit

    fun questpdf__decoration_descriptor__after__4cf66f67(target: Pointer): Pointer

    fun questpdf__decoration_descriptor__after__4c35dd57(target: Pointer, handler: Callback): Unit



}

// ============================================================================
// DecorationDescriptor Callback Interfaces
// ============================================================================


/**
 * Callback interface for Container configuration.
 */
interface ContainerCallback : Callback {
    fun callback(ptr: Pointer)
}


// ============================================================================
// DecorationDescriptor Class
// ============================================================================

/**
 * DecorationDescriptor wrapper for the native QuestPDF DecorationDescriptor.
 */
class DecorationDescriptor internal constructor(val pointer: Pointer) {
    private val callbacks = mutableListOf<Callback>()

    companion object {
        private var nativeLib: DecorationDescriptorNative? = QuestLibrary.getLibrary(DecorationDescriptorNative::class.java)
        private val initLock = Any()

        private fun lib(): DecorationDescriptorNative {
            return nativeLib!!
        }

    
        
    
        
    
        
    
        
    
        
    
        
    
    }
    

 


fun before(): Container {


    val result = lib().questpdf__decoration_descriptor__before__1bfecdf8(pointer)
    return Container(result)

}
 

 


fun before(handler: (Container) -> Unit) {

    // Create callback for handler
    val handlerCallback = object : ContainerCallback {
    override fun callback(ptr: Pointer) {
    val obj = Container(ptr)
    handler(obj)
    }
    }
    callbacks.add(handlerCallback)



    lib().questpdf__decoration_descriptor__before__bf5ce29e(pointer, handlerCallback)

}
 

 


fun content(): Container {


    val result = lib().questpdf__decoration_descriptor__content__9ec35667(pointer)
    return Container(result)

}
 

 


fun content(handler: (Container) -> Unit) {

    // Create callback for handler
    val handlerCallback = object : ContainerCallback {
    override fun callback(ptr: Pointer) {
    val obj = Container(ptr)
    handler(obj)
    }
    }
    callbacks.add(handlerCallback)



    lib().questpdf__decoration_descriptor__content__391a971a(pointer, handlerCallback)

}
 

 


fun after(): Container {


    val result = lib().questpdf__decoration_descriptor__after__4cf66f67(pointer)
    return Container(result)

}
 

 


fun after(handler: (Container) -> Unit) {

    // Create callback for handler
    val handlerCallback = object : ContainerCallback {
    override fun callback(ptr: Pointer) {
    val obj = Container(ptr)
    handler(obj)
    }
    }
    callbacks.add(handlerCallback)



    lib().questpdf__decoration_descriptor__after__4c35dd57(pointer, handlerCallback)

}
 


    
}




// ============================================================================
// InlinedDescriptor - Native Function Interface
// ============================================================================



/**
 * JNA interface for InlinedDescriptor native methods.
 */
private interface InlinedDescriptorNative : Library {

    fun questpdf__inlined_descriptor__spacing__e466eaa7(target: Pointer, value: Float, unit: Int): Unit

    fun questpdf__inlined_descriptor__vertical_spacing__44456280(target: Pointer, value: Float, unit: Int): Unit

    fun questpdf__inlined_descriptor__horizontal_spacing__a035fbb4(target: Pointer, value: Float, unit: Int): Unit

    fun questpdf__inlined_descriptor__baseline_top__96b48f7f(target: Pointer): Unit

    fun questpdf__inlined_descriptor__baseline_middle__2ee97366(target: Pointer): Unit

    fun questpdf__inlined_descriptor__baseline_bottom__1878876e(target: Pointer): Unit

    fun questpdf__inlined_descriptor__align_left__0c3a1762(target: Pointer): Unit

    fun questpdf__inlined_descriptor__align_center__d09c92f2(target: Pointer): Unit

    fun questpdf__inlined_descriptor__align_right__99b3ac01(target: Pointer): Unit

    fun questpdf__inlined_descriptor__align_justify__3f036912(target: Pointer): Unit

    fun questpdf__inlined_descriptor__align_space_around__cfaed88d(target: Pointer): Unit

    fun questpdf__inlined_descriptor__item__3a4e6d7b(target: Pointer): Pointer



}

// ============================================================================
// InlinedDescriptor Callback Interfaces
// ============================================================================


// ============================================================================
// InlinedDescriptor Class
// ============================================================================

/**
 * InlinedDescriptor wrapper for the native QuestPDF InlinedDescriptor.
 */
class InlinedDescriptor internal constructor(val pointer: Pointer) {
    private val callbacks = mutableListOf<Callback>()

    companion object {
        private var nativeLib: InlinedDescriptorNative? = QuestLibrary.getLibrary(InlinedDescriptorNative::class.java)
        private val initLock = Any()

        private fun lib(): InlinedDescriptorNative {
            return nativeLib!!
        }

    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
    }
    

 


fun spacing(value: Float, unit: LengthUnit = LengthUnit.Point) {


    lib().questpdf__inlined_descriptor__spacing__e466eaa7(pointer, value, unit.value)

}
 

 


fun verticalSpacing(value: Float, unit: LengthUnit = LengthUnit.Point) {


    lib().questpdf__inlined_descriptor__vertical_spacing__44456280(pointer, value, unit.value)

}
 

 


fun horizontalSpacing(value: Float, unit: LengthUnit = LengthUnit.Point) {


    lib().questpdf__inlined_descriptor__horizontal_spacing__a035fbb4(pointer, value, unit.value)

}
 

 


fun baselineTop() {


    lib().questpdf__inlined_descriptor__baseline_top__96b48f7f(pointer)

}
 

 


fun baselineMiddle() {


    lib().questpdf__inlined_descriptor__baseline_middle__2ee97366(pointer)

}
 

 


fun baselineBottom() {


    lib().questpdf__inlined_descriptor__baseline_bottom__1878876e(pointer)

}
 

 


fun alignLeft() {


    lib().questpdf__inlined_descriptor__align_left__0c3a1762(pointer)

}
 

 


fun alignCenter() {


    lib().questpdf__inlined_descriptor__align_center__d09c92f2(pointer)

}
 

 


fun alignRight() {


    lib().questpdf__inlined_descriptor__align_right__99b3ac01(pointer)

}
 

 


fun alignJustify() {


    lib().questpdf__inlined_descriptor__align_justify__3f036912(pointer)

}
 

 


fun alignSpaceAround() {


    lib().questpdf__inlined_descriptor__align_space_around__cfaed88d(pointer)

}
 

 


fun item(): Container {


    val result = lib().questpdf__inlined_descriptor__item__3a4e6d7b(pointer)
    return Container(result)

}
 


    
}




// ============================================================================
// LayersDescriptor - Native Function Interface
// ============================================================================



/**
 * JNA interface for LayersDescriptor native methods.
 */
private interface LayersDescriptorNative : Library {

    fun questpdf__layers_descriptor__layer__f8c1dd4f(target: Pointer): Pointer

    fun questpdf__layers_descriptor__primary_layer__c2eb4a19(target: Pointer): Pointer



}

// ============================================================================
// LayersDescriptor Callback Interfaces
// ============================================================================


// ============================================================================
// LayersDescriptor Class
// ============================================================================

/**
 * LayersDescriptor wrapper for the native QuestPDF LayersDescriptor.
 */
class LayersDescriptor internal constructor(val pointer: Pointer) {
    private val callbacks = mutableListOf<Callback>()

    companion object {
        private var nativeLib: LayersDescriptorNative? = QuestLibrary.getLibrary(LayersDescriptorNative::class.java)
        private val initLock = Any()

        private fun lib(): LayersDescriptorNative {
            return nativeLib!!
        }

    
        
    
        
    
    }
    

 


fun layer(): Container {


    val result = lib().questpdf__layers_descriptor__layer__f8c1dd4f(pointer)
    return Container(result)

}
 

 


fun primaryLayer(): Container {


    val result = lib().questpdf__layers_descriptor__primary_layer__c2eb4a19(pointer)
    return Container(result)

}
 


    
}




// ============================================================================
// RowDescriptor - Native Function Interface
// ============================================================================



/**
 * JNA interface for RowDescriptor native methods.
 */
private interface RowDescriptorNative : Library {

    fun questpdf__row_descriptor__spacing__09cc7a62(target: Pointer, spacing: Float, unit: Int): Unit

    fun questpdf__row_descriptor__relative_item__f4570b47(target: Pointer, size: Float): Pointer

    fun questpdf__row_descriptor__constant_item__4f927836(target: Pointer, size: Float, unit: Int): Pointer

    fun questpdf__row_descriptor__auto_item__fc084be8(target: Pointer): Pointer



}

// ============================================================================
// RowDescriptor Callback Interfaces
// ============================================================================


// ============================================================================
// RowDescriptor Class
// ============================================================================

/**
 * RowDescriptor wrapper for the native QuestPDF RowDescriptor.
 */
class RowDescriptor internal constructor(val pointer: Pointer) {
    private val callbacks = mutableListOf<Callback>()

    companion object {
        private var nativeLib: RowDescriptorNative? = QuestLibrary.getLibrary(RowDescriptorNative::class.java)
        private val initLock = Any()

        private fun lib(): RowDescriptorNative {
            return nativeLib!!
        }

    
        
    
        
    
        
    
        
    
    }
    

 


fun spacing(spacing: Float, unit: LengthUnit = LengthUnit.Point) {


    lib().questpdf__row_descriptor__spacing__09cc7a62(pointer, spacing, unit.value)

}
 

 


fun relativeItem(size: Float = 1f): Container {


    val result = lib().questpdf__row_descriptor__relative_item__f4570b47(pointer, size)
    return Container(result)

}
 

 


fun constantItem(size: Float, unit: LengthUnit = LengthUnit.Point): Container {


    val result = lib().questpdf__row_descriptor__constant_item__4f927836(pointer, size, unit.value)
    return Container(result)

}
 

 


fun autoItem(): Container {


    val result = lib().questpdf__row_descriptor__auto_item__fc084be8(pointer)
    return Container(result)

}
 


    
}




// ============================================================================
// GridDescriptor - Native Function Interface
// ============================================================================



/**
 * JNA interface for GridDescriptor native methods.
 */
private interface GridDescriptorNative : Library {

    fun questpdf__grid_descriptor__spacing__2a69d201(target: Pointer, value: Float, unit: Int): Unit

    fun questpdf__grid_descriptor__vertical_spacing__593ca4c3(target: Pointer, value: Float, unit: Int): Unit

    fun questpdf__grid_descriptor__horizontal_spacing__a9d6ceae(target: Pointer, value: Float, unit: Int): Unit

    fun questpdf__grid_descriptor__columns__160f5f35(target: Pointer, value: Int): Unit

    fun questpdf__grid_descriptor__align_left__fc5e4cb9(target: Pointer): Unit

    fun questpdf__grid_descriptor__align_center__3d81b2fe(target: Pointer): Unit

    fun questpdf__grid_descriptor__align_right__e9aa71bc(target: Pointer): Unit

    fun questpdf__grid_descriptor__item__3e7cf6ba(target: Pointer, columns: Int): Pointer



}

// ============================================================================
// GridDescriptor Callback Interfaces
// ============================================================================


// ============================================================================
// GridDescriptor Class
// ============================================================================

/**
 * GridDescriptor wrapper for the native QuestPDF GridDescriptor.
 */
class GridDescriptor internal constructor(val pointer: Pointer) {
    private val callbacks = mutableListOf<Callback>()

    companion object {
        private var nativeLib: GridDescriptorNative? = QuestLibrary.getLibrary(GridDescriptorNative::class.java)
        private val initLock = Any()

        private fun lib(): GridDescriptorNative {
            return nativeLib!!
        }

    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
    }
    

 


fun spacing(value: Float, unit: LengthUnit = LengthUnit.Point) {


    lib().questpdf__grid_descriptor__spacing__2a69d201(pointer, value, unit.value)

}
 

 


fun verticalSpacing(value: Float, unit: LengthUnit = LengthUnit.Point) {


    lib().questpdf__grid_descriptor__vertical_spacing__593ca4c3(pointer, value, unit.value)

}
 

 


fun horizontalSpacing(value: Float, unit: LengthUnit = LengthUnit.Point) {


    lib().questpdf__grid_descriptor__horizontal_spacing__a9d6ceae(pointer, value, unit.value)

}
 

 


fun columns(value: Int = 12) {


    lib().questpdf__grid_descriptor__columns__160f5f35(pointer, value)

}
 

 


fun alignLeft() {


    lib().questpdf__grid_descriptor__align_left__fc5e4cb9(pointer)

}
 

 


fun alignCenter() {


    lib().questpdf__grid_descriptor__align_center__3d81b2fe(pointer)

}
 

 


fun alignRight() {


    lib().questpdf__grid_descriptor__align_right__e9aa71bc(pointer)

}
 

 


fun item(columns: Int = 1): Container {


    val result = lib().questpdf__grid_descriptor__item__3e7cf6ba(pointer, columns)
    return Container(result)

}
 


    
}




// ============================================================================
// MultiColumnDescriptor - Native Function Interface
// ============================================================================



/**
 * JNA interface for MultiColumnDescriptor native methods.
 */
private interface MultiColumnDescriptorNative : Library {

    fun questpdf__multi_column_descriptor__spacing__b96a0ed7(target: Pointer, value: Float, unit: Int): Unit

    fun questpdf__multi_column_descriptor__columns__f9027e4e(target: Pointer, value: Int): Unit

    fun questpdf__multi_column_descriptor__balance_height__a0509325(target: Pointer, enable: Byte): Unit

    fun questpdf__multi_column_descriptor__content__68196264(target: Pointer): Pointer

    fun questpdf__multi_column_descriptor__spacer__9d6eea5d(target: Pointer): Pointer



}

// ============================================================================
// MultiColumnDescriptor Callback Interfaces
// ============================================================================


// ============================================================================
// MultiColumnDescriptor Class
// ============================================================================

/**
 * MultiColumnDescriptor wrapper for the native QuestPDF MultiColumnDescriptor.
 */
class MultiColumnDescriptor internal constructor(val pointer: Pointer) {
    private val callbacks = mutableListOf<Callback>()

    companion object {
        private var nativeLib: MultiColumnDescriptorNative? = QuestLibrary.getLibrary(MultiColumnDescriptorNative::class.java)
        private val initLock = Any()

        private fun lib(): MultiColumnDescriptorNative {
            return nativeLib!!
        }

    
        
    
        
    
        
    
        
    
        
    
    }
    

 


fun spacing(value: Float, unit: LengthUnit = LengthUnit.Point) {


    lib().questpdf__multi_column_descriptor__spacing__b96a0ed7(pointer, value, unit.value)

}
 

 


fun columns(value: Int = 2) {


    lib().questpdf__multi_column_descriptor__columns__f9027e4e(pointer, value)

}
 

 


fun balanceHeight(enable: Boolean = true) {


    lib().questpdf__multi_column_descriptor__balance_height__a0509325(pointer, (if (enable) 1.toByte() else 0.toByte()))

}
 

 


fun content(): Container {


    val result = lib().questpdf__multi_column_descriptor__content__68196264(pointer)
    return Container(result)

}
 

 


fun spacer(): Container {


    val result = lib().questpdf__multi_column_descriptor__spacer__9d6eea5d(pointer)
    return Container(result)

}
 


    
}




// ============================================================================
// TableCellDescriptor - Native Function Interface
// ============================================================================



/**
 * JNA interface for TableCellDescriptor native methods.
 */
private interface TableCellDescriptorNative : Library {

    fun questpdf__table_cell_descriptor__cell__1061edf9(target: Pointer): Pointer



}

// ============================================================================
// TableCellDescriptor Callback Interfaces
// ============================================================================


// ============================================================================
// TableCellDescriptor Class
// ============================================================================

/**
 * TableCellDescriptor wrapper for the native QuestPDF TableCellDescriptor.
 */
class TableCellDescriptor internal constructor(val pointer: Pointer) {
    private val callbacks = mutableListOf<Callback>()

    companion object {
        private var nativeLib: TableCellDescriptorNative? = QuestLibrary.getLibrary(TableCellDescriptorNative::class.java)
        private val initLock = Any()

        private fun lib(): TableCellDescriptorNative {
            return nativeLib!!
        }

    
        
    
    }
    

 


fun cell(): TableCellContainer {


    val result = lib().questpdf__table_cell_descriptor__cell__1061edf9(pointer)
    return TableCellContainer(result)

}
 


    
}




// ============================================================================
// TableColumnsDefinitionDescriptor - Native Function Interface
// ============================================================================



/**
 * JNA interface for TableColumnsDefinitionDescriptor native methods.
 */
private interface TableColumnsDefinitionDescriptorNative : Library {

    fun questpdf__table_columns_definition_descriptor__constant_column__e71e4979(target: Pointer, width: Float, unit: Int): Unit

    fun questpdf__table_columns_definition_descriptor__relative_column__940a67b1(target: Pointer, width: Float): Unit



}

// ============================================================================
// TableColumnsDefinitionDescriptor Callback Interfaces
// ============================================================================


// ============================================================================
// TableColumnsDefinitionDescriptor Class
// ============================================================================

/**
 * TableColumnsDefinitionDescriptor wrapper for the native QuestPDF TableColumnsDefinitionDescriptor.
 */
class TableColumnsDefinitionDescriptor internal constructor(val pointer: Pointer) {
    private val callbacks = mutableListOf<Callback>()

    companion object {
        private var nativeLib: TableColumnsDefinitionDescriptorNative? = QuestLibrary.getLibrary(TableColumnsDefinitionDescriptorNative::class.java)
        private val initLock = Any()

        private fun lib(): TableColumnsDefinitionDescriptorNative {
            return nativeLib!!
        }

    
        
    
        
    
    }
    

 


fun constantColumn(width: Float, unit: LengthUnit = LengthUnit.Point) {


    lib().questpdf__table_columns_definition_descriptor__constant_column__e71e4979(pointer, width, unit.value)

}
 

 


fun relativeColumn(width: Float = 1f) {


    lib().questpdf__table_columns_definition_descriptor__relative_column__940a67b1(pointer, width)

}
 


    
}




// ============================================================================
// TableDescriptor - Native Function Interface
// ============================================================================



/**
 * JNA interface for TableDescriptor native methods.
 */
private interface TableDescriptorNative : Library {

    fun questpdf__table_descriptor__columns_definition__1b198f41(target: Pointer, handler: Callback): Unit

    fun questpdf__table_descriptor__extend_last_cells_to_table_bottom__22a2235b(target: Pointer): Unit

    fun questpdf__table_descriptor__header__227448b3(target: Pointer, handler: Callback): Unit

    fun questpdf__table_descriptor__footer__a74a23a5(target: Pointer, handler: Callback): Unit

    fun questpdf__table_descriptor__cell__1f40892e(target: Pointer): Pointer



}

// ============================================================================
// TableDescriptor Callback Interfaces
// ============================================================================


/**
 * Callback interface for TableColumnsDefinitionDescriptor configuration.
 */
interface TableColumnsDefinitionDescriptorCallback : Callback {
    fun callback(ptr: Pointer)
}


/**
 * Callback interface for TableCellDescriptor configuration.
 */
interface TableCellDescriptorCallback : Callback {
    fun callback(ptr: Pointer)
}


// ============================================================================
// TableDescriptor Class
// ============================================================================

/**
 * TableDescriptor wrapper for the native QuestPDF TableDescriptor.
 */
class TableDescriptor internal constructor(val pointer: Pointer) {
    private val callbacks = mutableListOf<Callback>()

    companion object {
        private var nativeLib: TableDescriptorNative? = QuestLibrary.getLibrary(TableDescriptorNative::class.java)
        private val initLock = Any()

        private fun lib(): TableDescriptorNative {
            return nativeLib!!
        }

    
        
    
        
    
        
    
        
    
        
    
    }
    

 


fun columnsDefinition(handler: (TableColumnsDefinitionDescriptor) -> Unit) {

    // Create callback for handler
    val handlerCallback = object : TableColumnsDefinitionDescriptorCallback {
    override fun callback(ptr: Pointer) {
    val obj = TableColumnsDefinitionDescriptor(ptr)
    handler(obj)
    }
    }
    callbacks.add(handlerCallback)



    lib().questpdf__table_descriptor__columns_definition__1b198f41(pointer, handlerCallback)

}
 

 


fun extendLastCellsToTableBottom() {


    lib().questpdf__table_descriptor__extend_last_cells_to_table_bottom__22a2235b(pointer)

}
 

 


fun header(handler: (TableCellDescriptor) -> Unit) {

    // Create callback for handler
    val handlerCallback = object : TableCellDescriptorCallback {
    override fun callback(ptr: Pointer) {
    val obj = TableCellDescriptor(ptr)
    handler(obj)
    }
    }
    callbacks.add(handlerCallback)



    lib().questpdf__table_descriptor__header__227448b3(pointer, handlerCallback)

}
 

 


fun footer(handler: (TableCellDescriptor) -> Unit) {

    // Create callback for handler
    val handlerCallback = object : TableCellDescriptorCallback {
    override fun callback(ptr: Pointer) {
    val obj = TableCellDescriptor(ptr)
    handler(obj)
    }
    }
    callbacks.add(handlerCallback)



    lib().questpdf__table_descriptor__footer__a74a23a5(pointer, handlerCallback)

}
 

 


fun cell(): TableCellContainer {


    val result = lib().questpdf__table_descriptor__cell__1f40892e(pointer)
    return TableCellContainer(result)

}
 


    
}




// ============================================================================
// TextDescriptor - Native Function Interface
// ============================================================================



/**
 * JNA interface for TextDescriptor native methods.
 */
private interface TextDescriptorNative : Library {

    fun questpdf__text_descriptor__align_left__4a573634(target: Pointer): Unit

    fun questpdf__text_descriptor__align_center__def2b616(target: Pointer): Unit

    fun questpdf__text_descriptor__align_right__de6eaa17(target: Pointer): Unit

    fun questpdf__text_descriptor__justify__1501b0fa(target: Pointer): Unit

    fun questpdf__text_descriptor__align_start__947ba696(target: Pointer): Unit

    fun questpdf__text_descriptor__align_end__5aefafc5(target: Pointer): Unit

    fun questpdf__text_descriptor__clamp_lines__f1b02b03(target: Pointer, maxLines: Int, ellipsis: String): Unit

    fun questpdf__text_descriptor__paragraph_spacing__c3629bd6(target: Pointer, value: Float, unit: Int): Unit

    fun questpdf__text_descriptor__paragraph_first_line_indentation__414498e7(target: Pointer, value: Float, unit: Int): Unit

    fun questpdf__text_descriptor__span__41a383c0(target: Pointer, text: String): Pointer

    fun questpdf__text_descriptor__line__17db2520(target: Pointer, text: String): Pointer

    fun questpdf__text_descriptor__empty_line__70ae8fc0(target: Pointer): Pointer

    fun questpdf__text_descriptor__current_page_number__2097e179(target: Pointer): Pointer

    fun questpdf__text_descriptor__total_pages__604d3e19(target: Pointer): Pointer

    fun questpdf__text_descriptor__begin_page_number_of_section__340accfc(target: Pointer, sectionName: String): Pointer

    fun questpdf__text_descriptor__end_page_number_of_section__deee569a(target: Pointer, sectionName: String): Pointer

    fun questpdf__text_descriptor__page_number_within_section__51768233(target: Pointer, sectionName: String): Pointer

    fun questpdf__text_descriptor__total_pages_within_section__250c06e5(target: Pointer, sectionName: String): Pointer

    fun questpdf__text_descriptor__section_link__c9b32c1a(target: Pointer, text: String, sectionName: String): Pointer

    fun questpdf__text_descriptor__hyperlink__f38a28c7(target: Pointer, text: String, url: String): Pointer

    fun questpdf__text_descriptor__element__862752ab(target: Pointer, alignment: Int): Pointer

    fun questpdf__text_descriptor__element__ff63896d(target: Pointer, handler: Callback, alignment: Int): Unit



}

// ============================================================================
// TextDescriptor Callback Interfaces
// ============================================================================


/**
 * Callback interface for Container configuration.
 */
interface ContainerCallback : Callback {
    fun callback(ptr: Pointer)
}


// ============================================================================
// TextDescriptor Class
// ============================================================================

/**
 * TextDescriptor wrapper for the native QuestPDF TextDescriptor.
 */
class TextDescriptor internal constructor(val pointer: Pointer) {
    private val callbacks = mutableListOf<Callback>()

    companion object {
        private var nativeLib: TextDescriptorNative? = QuestLibrary.getLibrary(TextDescriptorNative::class.java)
        private val initLock = Any()

        private fun lib(): TextDescriptorNative {
            return nativeLib!!
        }

    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
    }
    

 


fun alignLeft() {


    lib().questpdf__text_descriptor__align_left__4a573634(pointer)

}
 

 


fun alignCenter() {


    lib().questpdf__text_descriptor__align_center__def2b616(pointer)

}
 

 


fun alignRight() {


    lib().questpdf__text_descriptor__align_right__de6eaa17(pointer)

}
 

 


fun justify() {


    lib().questpdf__text_descriptor__justify__1501b0fa(pointer)

}
 

 


fun alignStart() {


    lib().questpdf__text_descriptor__align_start__947ba696(pointer)

}
 

 


fun alignEnd() {


    lib().questpdf__text_descriptor__align_end__5aefafc5(pointer)

}
 

 


fun clampLines(maxLines: Int, ellipsis: String = "…") {


    lib().questpdf__text_descriptor__clamp_lines__f1b02b03(pointer, maxLines, ellipsis)

}
 

 


fun paragraphSpacing(value: Float, unit: LengthUnit = LengthUnit.Point) {


    lib().questpdf__text_descriptor__paragraph_spacing__c3629bd6(pointer, value, unit.value)

}
 

 


fun paragraphFirstLineIndentation(value: Float, unit: LengthUnit = LengthUnit.Point) {


    lib().questpdf__text_descriptor__paragraph_first_line_indentation__414498e7(pointer, value, unit.value)

}
 

 


fun span(text: String): TextSpanDescriptor {


    val result = lib().questpdf__text_descriptor__span__41a383c0(pointer, text)
    return TextSpanDescriptor(result)

}
 

 


fun line(text: String): TextSpanDescriptor {


    val result = lib().questpdf__text_descriptor__line__17db2520(pointer, text)
    return TextSpanDescriptor(result)

}
 

 


fun emptyLine(): TextSpanDescriptor {


    val result = lib().questpdf__text_descriptor__empty_line__70ae8fc0(pointer)
    return TextSpanDescriptor(result)

}
 

 


fun currentPageNumber(): TextPageNumberDescriptor {


    val result = lib().questpdf__text_descriptor__current_page_number__2097e179(pointer)
    return TextPageNumberDescriptor(result)

}
 

 


fun totalPages(): TextPageNumberDescriptor {


    val result = lib().questpdf__text_descriptor__total_pages__604d3e19(pointer)
    return TextPageNumberDescriptor(result)

}
 

 


fun beginPageNumberOfSection(sectionName: String): TextPageNumberDescriptor {


    val result = lib().questpdf__text_descriptor__begin_page_number_of_section__340accfc(pointer, sectionName)
    return TextPageNumberDescriptor(result)

}
 

 


fun endPageNumberOfSection(sectionName: String): TextPageNumberDescriptor {


    val result = lib().questpdf__text_descriptor__end_page_number_of_section__deee569a(pointer, sectionName)
    return TextPageNumberDescriptor(result)

}
 

 


fun pageNumberWithinSection(sectionName: String): TextPageNumberDescriptor {


    val result = lib().questpdf__text_descriptor__page_number_within_section__51768233(pointer, sectionName)
    return TextPageNumberDescriptor(result)

}
 

 


fun totalPagesWithinSection(sectionName: String): TextPageNumberDescriptor {


    val result = lib().questpdf__text_descriptor__total_pages_within_section__250c06e5(pointer, sectionName)
    return TextPageNumberDescriptor(result)

}
 

 


fun sectionLink(text: String, sectionName: String): TextSpanDescriptor {


    val result = lib().questpdf__text_descriptor__section_link__c9b32c1a(pointer, text, sectionName)
    return TextSpanDescriptor(result)

}
 

 


fun hyperlink(text: String, url: String): TextSpanDescriptor {


    val result = lib().questpdf__text_descriptor__hyperlink__f38a28c7(pointer, text, url)
    return TextSpanDescriptor(result)

}
 

 


fun element(alignment: TextInjectedElementAlignment = TextInjectedElementAlignment.AboveBaseline): Container {


    val result = lib().questpdf__text_descriptor__element__862752ab(pointer, alignment.value)
    return Container(result)

}
 

 


fun element(handler: (Container) -> Unit, alignment: TextInjectedElementAlignment = TextInjectedElementAlignment.AboveBaseline) {

    // Create callback for handler
    val handlerCallback = object : ContainerCallback {
    override fun callback(ptr: Pointer) {
    val obj = Container(ptr)
    handler(obj)
    }
    }
    callbacks.add(handlerCallback)



    lib().questpdf__text_descriptor__element__ff63896d(pointer, handlerCallback, alignment.value)

}
 


    
}




// ============================================================================
// TextSpanDescriptor - Native Function Interface
// ============================================================================



/**
 * JNA interface for TextSpanDescriptor native methods.
 */
private interface TextSpanDescriptorNative : Library {

    fun questpdf__text_span_descriptor__font_color__a0d06e42(target: Pointer, color: Int): Pointer

    fun questpdf__text_span_descriptor__background_color__5461b453(target: Pointer, color: Int): Pointer

    fun questpdf__text_span_descriptor__font_size__c989487d(target: Pointer, value: Float): Pointer

    fun questpdf__text_span_descriptor__line_height__a1b4697a(target: Pointer, factor: Pointer): Pointer

    fun questpdf__text_span_descriptor__letter_spacing__92f86a26(target: Pointer, factor: Float): Pointer

    fun questpdf__text_span_descriptor__word_spacing__1f794add(target: Pointer, factor: Float): Pointer

    fun questpdf__text_span_descriptor__italic__4f023aba(target: Pointer, value: Byte): Pointer

    fun questpdf__text_span_descriptor__strikethrough__41841206(target: Pointer, value: Byte): Pointer

    fun questpdf__text_span_descriptor__underline__2e1ae473(target: Pointer, value: Byte): Pointer

    fun questpdf__text_span_descriptor__overline__add25860(target: Pointer, value: Byte): Pointer

    fun questpdf__text_span_descriptor__decoration_color__5d18d151(target: Pointer, color: Int): Pointer

    fun questpdf__text_span_descriptor__decoration_thickness__c7c23c84(target: Pointer, factor: Float): Pointer

    fun questpdf__text_span_descriptor__decoration_solid__f64746d1(target: Pointer): Pointer

    fun questpdf__text_span_descriptor__decoration_double__41cf8a18(target: Pointer): Pointer

    fun questpdf__text_span_descriptor__decoration_wavy__1761acf2(target: Pointer): Pointer

    fun questpdf__text_span_descriptor__decoration_dotted__e940537a(target: Pointer): Pointer

    fun questpdf__text_span_descriptor__decoration_dashed__a85f7344(target: Pointer): Pointer

    fun questpdf__text_span_descriptor__thin__e9036638(target: Pointer): Pointer

    fun questpdf__text_span_descriptor__extra_light__33bbe020(target: Pointer): Pointer

    fun questpdf__text_span_descriptor__light__37ef1bc2(target: Pointer): Pointer

    fun questpdf__text_span_descriptor__normal_weight__18d360b3(target: Pointer): Pointer

    fun questpdf__text_span_descriptor__medium__5ef8b80e(target: Pointer): Pointer

    fun questpdf__text_span_descriptor__semi_bold__0b92f7b7(target: Pointer): Pointer

    fun questpdf__text_span_descriptor__bold__0dfa9061(target: Pointer): Pointer

    fun questpdf__text_span_descriptor__extra_bold__c4fbc0a6(target: Pointer): Pointer

    fun questpdf__text_span_descriptor__black__0cc8d698(target: Pointer): Pointer

    fun questpdf__text_span_descriptor__extra_black__c7698d85(target: Pointer): Pointer

    fun questpdf__text_span_descriptor__normal_position__5e5176cb(target: Pointer): Pointer

    fun questpdf__text_span_descriptor__subscript__db9bd4eb(target: Pointer): Pointer

    fun questpdf__text_span_descriptor__superscript__a9b46b1e(target: Pointer): Pointer

    fun questpdf__text_span_descriptor__direction_auto__fbed9e71(target: Pointer): Pointer

    fun questpdf__text_span_descriptor__direction_from_left_to_right__09e2e3bc(target: Pointer): Pointer

    fun questpdf__text_span_descriptor__direction_from_right_to_left__6cc5cb9e(target: Pointer): Pointer

    fun questpdf__text_span_descriptor__enable_font_feature__136a164d(target: Pointer, featureName: String): Pointer

    fun questpdf__text_span_descriptor__disable_font_feature__5bd81de9(target: Pointer, featureName: String): Pointer



}

// ============================================================================
// TextSpanDescriptor Callback Interfaces
// ============================================================================


// ============================================================================
// TextSpanDescriptor Class
// ============================================================================

/**
 * TextSpanDescriptor wrapper for the native QuestPDF TextSpanDescriptor.
 */
class TextSpanDescriptor internal constructor(val pointer: Pointer) {
    private val callbacks = mutableListOf<Callback>()

    companion object {
        private var nativeLib: TextSpanDescriptorNative? = QuestLibrary.getLibrary(TextSpanDescriptorNative::class.java)
        private val initLock = Any()

        private fun lib(): TextSpanDescriptorNative {
            return nativeLib!!
        }

    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
    }
    

 


fun fontColor(color: Color): TextSpanDescriptor {


    val result = lib().questpdf__text_span_descriptor__font_color__a0d06e42(pointer, color.hex.toInt())
    return TextSpanDescriptor(result)

}
 

 


fun backgroundColor(color: Color): TextSpanDescriptor {


    val result = lib().questpdf__text_span_descriptor__background_color__5461b453(pointer, color.hex.toInt())
    return TextSpanDescriptor(result)

}
 

 


fun fontSize(value: Float): TextSpanDescriptor {


    val result = lib().questpdf__text_span_descriptor__font_size__c989487d(pointer, value)
    return TextSpanDescriptor(result)

}
 

 


fun lineHeight(factor: Any): TextSpanDescriptor {


    val result = lib().questpdf__text_span_descriptor__line_height__a1b4697a(pointer, factor)
    return TextSpanDescriptor(result)

}
 

 


fun letterSpacing(factor: Float = 0f): TextSpanDescriptor {


    val result = lib().questpdf__text_span_descriptor__letter_spacing__92f86a26(pointer, factor)
    return TextSpanDescriptor(result)

}
 

 


fun wordSpacing(factor: Float = 0f): TextSpanDescriptor {


    val result = lib().questpdf__text_span_descriptor__word_spacing__1f794add(pointer, factor)
    return TextSpanDescriptor(result)

}
 

 


fun italic(value: Boolean = true): TextSpanDescriptor {


    val result = lib().questpdf__text_span_descriptor__italic__4f023aba(pointer, (if (value) 1.toByte() else 0.toByte()))
    return TextSpanDescriptor(result)

}
 

 


fun strikethrough(value: Boolean = true): TextSpanDescriptor {


    val result = lib().questpdf__text_span_descriptor__strikethrough__41841206(pointer, (if (value) 1.toByte() else 0.toByte()))
    return TextSpanDescriptor(result)

}
 

 


fun underline(value: Boolean = true): TextSpanDescriptor {


    val result = lib().questpdf__text_span_descriptor__underline__2e1ae473(pointer, (if (value) 1.toByte() else 0.toByte()))
    return TextSpanDescriptor(result)

}
 

 


fun overline(value: Boolean = true): TextSpanDescriptor {


    val result = lib().questpdf__text_span_descriptor__overline__add25860(pointer, (if (value) 1.toByte() else 0.toByte()))
    return TextSpanDescriptor(result)

}
 

 


fun decorationColor(color: Color): TextSpanDescriptor {


    val result = lib().questpdf__text_span_descriptor__decoration_color__5d18d151(pointer, color.hex.toInt())
    return TextSpanDescriptor(result)

}
 

 


fun decorationThickness(factor: Float): TextSpanDescriptor {


    val result = lib().questpdf__text_span_descriptor__decoration_thickness__c7c23c84(pointer, factor)
    return TextSpanDescriptor(result)

}
 

 


fun decorationSolid(): TextSpanDescriptor {


    val result = lib().questpdf__text_span_descriptor__decoration_solid__f64746d1(pointer)
    return TextSpanDescriptor(result)

}
 

 


fun decorationDouble(): TextSpanDescriptor {


    val result = lib().questpdf__text_span_descriptor__decoration_double__41cf8a18(pointer)
    return TextSpanDescriptor(result)

}
 

 


fun decorationWavy(): TextSpanDescriptor {


    val result = lib().questpdf__text_span_descriptor__decoration_wavy__1761acf2(pointer)
    return TextSpanDescriptor(result)

}
 

 


fun decorationDotted(): TextSpanDescriptor {


    val result = lib().questpdf__text_span_descriptor__decoration_dotted__e940537a(pointer)
    return TextSpanDescriptor(result)

}
 

 


fun decorationDashed(): TextSpanDescriptor {


    val result = lib().questpdf__text_span_descriptor__decoration_dashed__a85f7344(pointer)
    return TextSpanDescriptor(result)

}
 

 


fun thin(): TextSpanDescriptor {


    val result = lib().questpdf__text_span_descriptor__thin__e9036638(pointer)
    return TextSpanDescriptor(result)

}
 

 


fun extraLight(): TextSpanDescriptor {


    val result = lib().questpdf__text_span_descriptor__extra_light__33bbe020(pointer)
    return TextSpanDescriptor(result)

}
 

 


fun light(): TextSpanDescriptor {


    val result = lib().questpdf__text_span_descriptor__light__37ef1bc2(pointer)
    return TextSpanDescriptor(result)

}
 

 


fun normalWeight(): TextSpanDescriptor {


    val result = lib().questpdf__text_span_descriptor__normal_weight__18d360b3(pointer)
    return TextSpanDescriptor(result)

}
 

 


fun medium(): TextSpanDescriptor {


    val result = lib().questpdf__text_span_descriptor__medium__5ef8b80e(pointer)
    return TextSpanDescriptor(result)

}
 

 


fun semiBold(): TextSpanDescriptor {


    val result = lib().questpdf__text_span_descriptor__semi_bold__0b92f7b7(pointer)
    return TextSpanDescriptor(result)

}
 

 


fun bold(): TextSpanDescriptor {


    val result = lib().questpdf__text_span_descriptor__bold__0dfa9061(pointer)
    return TextSpanDescriptor(result)

}
 

 


fun extraBold(): TextSpanDescriptor {


    val result = lib().questpdf__text_span_descriptor__extra_bold__c4fbc0a6(pointer)
    return TextSpanDescriptor(result)

}
 

 


fun black(): TextSpanDescriptor {


    val result = lib().questpdf__text_span_descriptor__black__0cc8d698(pointer)
    return TextSpanDescriptor(result)

}
 

 


fun extraBlack(): TextSpanDescriptor {


    val result = lib().questpdf__text_span_descriptor__extra_black__c7698d85(pointer)
    return TextSpanDescriptor(result)

}
 

 


fun normalPosition(): TextSpanDescriptor {


    val result = lib().questpdf__text_span_descriptor__normal_position__5e5176cb(pointer)
    return TextSpanDescriptor(result)

}
 

 


fun subscript(): TextSpanDescriptor {


    val result = lib().questpdf__text_span_descriptor__subscript__db9bd4eb(pointer)
    return TextSpanDescriptor(result)

}
 

 


fun superscript(): TextSpanDescriptor {


    val result = lib().questpdf__text_span_descriptor__superscript__a9b46b1e(pointer)
    return TextSpanDescriptor(result)

}
 

 


fun directionAuto(): TextSpanDescriptor {


    val result = lib().questpdf__text_span_descriptor__direction_auto__fbed9e71(pointer)
    return TextSpanDescriptor(result)

}
 

 


fun directionFromLeftToRight(): TextSpanDescriptor {


    val result = lib().questpdf__text_span_descriptor__direction_from_left_to_right__09e2e3bc(pointer)
    return TextSpanDescriptor(result)

}
 

 


fun directionFromRightToLeft(): TextSpanDescriptor {


    val result = lib().questpdf__text_span_descriptor__direction_from_right_to_left__6cc5cb9e(pointer)
    return TextSpanDescriptor(result)

}
 

 


fun enableFontFeature(featureName: String): TextSpanDescriptor {


    val result = lib().questpdf__text_span_descriptor__enable_font_feature__136a164d(pointer, featureName)
    return TextSpanDescriptor(result)

}
 

 


fun disableFontFeature(featureName: String): TextSpanDescriptor {


    val result = lib().questpdf__text_span_descriptor__disable_font_feature__5bd81de9(pointer, featureName)
    return TextSpanDescriptor(result)

}
 


    
}




// ============================================================================
// TextPageNumberDescriptor - Native Function Interface
// ============================================================================



/**
 * JNA interface for TextPageNumberDescriptor native methods.
 */
private interface TextPageNumberDescriptorNative : Library {

    fun questpdf__text_page_number_descriptor__font_color__a0d06e42(target: Pointer, color: Int): Pointer

    fun questpdf__text_page_number_descriptor__background_color__5461b453(target: Pointer, color: Int): Pointer

    fun questpdf__text_page_number_descriptor__font_size__c989487d(target: Pointer, value: Float): Pointer

    fun questpdf__text_page_number_descriptor__line_height__a1b4697a(target: Pointer, factor: Pointer): Pointer

    fun questpdf__text_page_number_descriptor__letter_spacing__92f86a26(target: Pointer, factor: Float): Pointer

    fun questpdf__text_page_number_descriptor__word_spacing__1f794add(target: Pointer, factor: Float): Pointer

    fun questpdf__text_page_number_descriptor__italic__4f023aba(target: Pointer, value: Byte): Pointer

    fun questpdf__text_page_number_descriptor__strikethrough__41841206(target: Pointer, value: Byte): Pointer

    fun questpdf__text_page_number_descriptor__underline__2e1ae473(target: Pointer, value: Byte): Pointer

    fun questpdf__text_page_number_descriptor__overline__add25860(target: Pointer, value: Byte): Pointer

    fun questpdf__text_page_number_descriptor__decoration_color__5d18d151(target: Pointer, color: Int): Pointer

    fun questpdf__text_page_number_descriptor__decoration_thickness__c7c23c84(target: Pointer, factor: Float): Pointer

    fun questpdf__text_page_number_descriptor__decoration_solid__f64746d1(target: Pointer): Pointer

    fun questpdf__text_page_number_descriptor__decoration_double__41cf8a18(target: Pointer): Pointer

    fun questpdf__text_page_number_descriptor__decoration_wavy__1761acf2(target: Pointer): Pointer

    fun questpdf__text_page_number_descriptor__decoration_dotted__e940537a(target: Pointer): Pointer

    fun questpdf__text_page_number_descriptor__decoration_dashed__a85f7344(target: Pointer): Pointer

    fun questpdf__text_page_number_descriptor__thin__e9036638(target: Pointer): Pointer

    fun questpdf__text_page_number_descriptor__extra_light__33bbe020(target: Pointer): Pointer

    fun questpdf__text_page_number_descriptor__light__37ef1bc2(target: Pointer): Pointer

    fun questpdf__text_page_number_descriptor__normal_weight__18d360b3(target: Pointer): Pointer

    fun questpdf__text_page_number_descriptor__medium__5ef8b80e(target: Pointer): Pointer

    fun questpdf__text_page_number_descriptor__semi_bold__0b92f7b7(target: Pointer): Pointer

    fun questpdf__text_page_number_descriptor__bold__0dfa9061(target: Pointer): Pointer

    fun questpdf__text_page_number_descriptor__extra_bold__c4fbc0a6(target: Pointer): Pointer

    fun questpdf__text_page_number_descriptor__black__0cc8d698(target: Pointer): Pointer

    fun questpdf__text_page_number_descriptor__extra_black__c7698d85(target: Pointer): Pointer

    fun questpdf__text_page_number_descriptor__normal_position__5e5176cb(target: Pointer): Pointer

    fun questpdf__text_page_number_descriptor__subscript__db9bd4eb(target: Pointer): Pointer

    fun questpdf__text_page_number_descriptor__superscript__a9b46b1e(target: Pointer): Pointer

    fun questpdf__text_page_number_descriptor__direction_auto__fbed9e71(target: Pointer): Pointer

    fun questpdf__text_page_number_descriptor__direction_from_left_to_right__09e2e3bc(target: Pointer): Pointer

    fun questpdf__text_page_number_descriptor__direction_from_right_to_left__6cc5cb9e(target: Pointer): Pointer

    fun questpdf__text_page_number_descriptor__enable_font_feature__136a164d(target: Pointer, featureName: String): Pointer

    fun questpdf__text_page_number_descriptor__disable_font_feature__5bd81de9(target: Pointer, featureName: String): Pointer



}

// ============================================================================
// TextPageNumberDescriptor Callback Interfaces
// ============================================================================


// ============================================================================
// TextPageNumberDescriptor Class
// ============================================================================

/**
 * TextPageNumberDescriptor wrapper for the native QuestPDF TextPageNumberDescriptor.
 */
class TextPageNumberDescriptor internal constructor(val pointer: Pointer) {
    private val callbacks = mutableListOf<Callback>()

    companion object {
        private var nativeLib: TextPageNumberDescriptorNative? = QuestLibrary.getLibrary(TextPageNumberDescriptorNative::class.java)
        private val initLock = Any()

        private fun lib(): TextPageNumberDescriptorNative {
            return nativeLib!!
        }

    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
    }
    

 


fun fontColor(color: Color): TextPageNumberDescriptor {


    val result = lib().questpdf__text_page_number_descriptor__font_color__a0d06e42(pointer, color.hex.toInt())
    return TextPageNumberDescriptor(result)

}
 

 


fun backgroundColor(color: Color): TextPageNumberDescriptor {


    val result = lib().questpdf__text_page_number_descriptor__background_color__5461b453(pointer, color.hex.toInt())
    return TextPageNumberDescriptor(result)

}
 

 


fun fontSize(value: Float): TextPageNumberDescriptor {


    val result = lib().questpdf__text_page_number_descriptor__font_size__c989487d(pointer, value)
    return TextPageNumberDescriptor(result)

}
 

 


fun lineHeight(factor: Any): TextPageNumberDescriptor {


    val result = lib().questpdf__text_page_number_descriptor__line_height__a1b4697a(pointer, factor)
    return TextPageNumberDescriptor(result)

}
 

 


fun letterSpacing(factor: Float = 0f): TextPageNumberDescriptor {


    val result = lib().questpdf__text_page_number_descriptor__letter_spacing__92f86a26(pointer, factor)
    return TextPageNumberDescriptor(result)

}
 

 


fun wordSpacing(factor: Float = 0f): TextPageNumberDescriptor {


    val result = lib().questpdf__text_page_number_descriptor__word_spacing__1f794add(pointer, factor)
    return TextPageNumberDescriptor(result)

}
 

 


fun italic(value: Boolean = true): TextPageNumberDescriptor {


    val result = lib().questpdf__text_page_number_descriptor__italic__4f023aba(pointer, (if (value) 1.toByte() else 0.toByte()))
    return TextPageNumberDescriptor(result)

}
 

 


fun strikethrough(value: Boolean = true): TextPageNumberDescriptor {


    val result = lib().questpdf__text_page_number_descriptor__strikethrough__41841206(pointer, (if (value) 1.toByte() else 0.toByte()))
    return TextPageNumberDescriptor(result)

}
 

 


fun underline(value: Boolean = true): TextPageNumberDescriptor {


    val result = lib().questpdf__text_page_number_descriptor__underline__2e1ae473(pointer, (if (value) 1.toByte() else 0.toByte()))
    return TextPageNumberDescriptor(result)

}
 

 


fun overline(value: Boolean = true): TextPageNumberDescriptor {


    val result = lib().questpdf__text_page_number_descriptor__overline__add25860(pointer, (if (value) 1.toByte() else 0.toByte()))
    return TextPageNumberDescriptor(result)

}
 

 


fun decorationColor(color: Color): TextPageNumberDescriptor {


    val result = lib().questpdf__text_page_number_descriptor__decoration_color__5d18d151(pointer, color.hex.toInt())
    return TextPageNumberDescriptor(result)

}
 

 


fun decorationThickness(factor: Float): TextPageNumberDescriptor {


    val result = lib().questpdf__text_page_number_descriptor__decoration_thickness__c7c23c84(pointer, factor)
    return TextPageNumberDescriptor(result)

}
 

 


fun decorationSolid(): TextPageNumberDescriptor {


    val result = lib().questpdf__text_page_number_descriptor__decoration_solid__f64746d1(pointer)
    return TextPageNumberDescriptor(result)

}
 

 


fun decorationDouble(): TextPageNumberDescriptor {


    val result = lib().questpdf__text_page_number_descriptor__decoration_double__41cf8a18(pointer)
    return TextPageNumberDescriptor(result)

}
 

 


fun decorationWavy(): TextPageNumberDescriptor {


    val result = lib().questpdf__text_page_number_descriptor__decoration_wavy__1761acf2(pointer)
    return TextPageNumberDescriptor(result)

}
 

 


fun decorationDotted(): TextPageNumberDescriptor {


    val result = lib().questpdf__text_page_number_descriptor__decoration_dotted__e940537a(pointer)
    return TextPageNumberDescriptor(result)

}
 

 


fun decorationDashed(): TextPageNumberDescriptor {


    val result = lib().questpdf__text_page_number_descriptor__decoration_dashed__a85f7344(pointer)
    return TextPageNumberDescriptor(result)

}
 

 


fun thin(): TextPageNumberDescriptor {


    val result = lib().questpdf__text_page_number_descriptor__thin__e9036638(pointer)
    return TextPageNumberDescriptor(result)

}
 

 


fun extraLight(): TextPageNumberDescriptor {


    val result = lib().questpdf__text_page_number_descriptor__extra_light__33bbe020(pointer)
    return TextPageNumberDescriptor(result)

}
 

 


fun light(): TextPageNumberDescriptor {


    val result = lib().questpdf__text_page_number_descriptor__light__37ef1bc2(pointer)
    return TextPageNumberDescriptor(result)

}
 

 


fun normalWeight(): TextPageNumberDescriptor {


    val result = lib().questpdf__text_page_number_descriptor__normal_weight__18d360b3(pointer)
    return TextPageNumberDescriptor(result)

}
 

 


fun medium(): TextPageNumberDescriptor {


    val result = lib().questpdf__text_page_number_descriptor__medium__5ef8b80e(pointer)
    return TextPageNumberDescriptor(result)

}
 

 


fun semiBold(): TextPageNumberDescriptor {


    val result = lib().questpdf__text_page_number_descriptor__semi_bold__0b92f7b7(pointer)
    return TextPageNumberDescriptor(result)

}
 

 


fun bold(): TextPageNumberDescriptor {


    val result = lib().questpdf__text_page_number_descriptor__bold__0dfa9061(pointer)
    return TextPageNumberDescriptor(result)

}
 

 


fun extraBold(): TextPageNumberDescriptor {


    val result = lib().questpdf__text_page_number_descriptor__extra_bold__c4fbc0a6(pointer)
    return TextPageNumberDescriptor(result)

}
 

 


fun black(): TextPageNumberDescriptor {


    val result = lib().questpdf__text_page_number_descriptor__black__0cc8d698(pointer)
    return TextPageNumberDescriptor(result)

}
 

 


fun extraBlack(): TextPageNumberDescriptor {


    val result = lib().questpdf__text_page_number_descriptor__extra_black__c7698d85(pointer)
    return TextPageNumberDescriptor(result)

}
 

 


fun normalPosition(): TextPageNumberDescriptor {


    val result = lib().questpdf__text_page_number_descriptor__normal_position__5e5176cb(pointer)
    return TextPageNumberDescriptor(result)

}
 

 


fun subscript(): TextPageNumberDescriptor {


    val result = lib().questpdf__text_page_number_descriptor__subscript__db9bd4eb(pointer)
    return TextPageNumberDescriptor(result)

}
 

 


fun superscript(): TextPageNumberDescriptor {


    val result = lib().questpdf__text_page_number_descriptor__superscript__a9b46b1e(pointer)
    return TextPageNumberDescriptor(result)

}
 

 


fun directionAuto(): TextPageNumberDescriptor {


    val result = lib().questpdf__text_page_number_descriptor__direction_auto__fbed9e71(pointer)
    return TextPageNumberDescriptor(result)

}
 

 


fun directionFromLeftToRight(): TextPageNumberDescriptor {


    val result = lib().questpdf__text_page_number_descriptor__direction_from_left_to_right__09e2e3bc(pointer)
    return TextPageNumberDescriptor(result)

}
 

 


fun directionFromRightToLeft(): TextPageNumberDescriptor {


    val result = lib().questpdf__text_page_number_descriptor__direction_from_right_to_left__6cc5cb9e(pointer)
    return TextPageNumberDescriptor(result)

}
 

 


fun enableFontFeature(featureName: String): TextPageNumberDescriptor {


    val result = lib().questpdf__text_page_number_descriptor__enable_font_feature__136a164d(pointer, featureName)
    return TextPageNumberDescriptor(result)

}
 

 


fun disableFontFeature(featureName: String): TextPageNumberDescriptor {


    val result = lib().questpdf__text_page_number_descriptor__disable_font_feature__5bd81de9(pointer, featureName)
    return TextPageNumberDescriptor(result)

}
 


    
}




// ============================================================================
// TextBlockDescriptor - Native Function Interface
// ============================================================================



/**
 * JNA interface for TextBlockDescriptor native methods.
 */
private interface TextBlockDescriptorNative : Library {

    fun questpdf__text_block_descriptor__align_left__da139fee(target: Pointer): Pointer

    fun questpdf__text_block_descriptor__align_center__3661d942(target: Pointer): Pointer

    fun questpdf__text_block_descriptor__align_right__28e79232(target: Pointer): Pointer

    fun questpdf__text_block_descriptor__justify__f4a5d951(target: Pointer): Pointer

    fun questpdf__text_block_descriptor__align_start__c97cfc2b(target: Pointer): Pointer

    fun questpdf__text_block_descriptor__align_end__e0ace0c1(target: Pointer): Pointer

    fun questpdf__text_block_descriptor__clamp_lines__2a5db05c(target: Pointer, maxLines: Int, ellipsis: String): Pointer

    fun questpdf__text_block_descriptor__paragraph_spacing__6dcddcea(target: Pointer, value: Float, unit: Int): Pointer

    fun questpdf__text_block_descriptor__paragraph_first_line_indentation__5d11cadd(target: Pointer, value: Float, unit: Int): Pointer

    fun questpdf__text_block_descriptor__font_color__a0d06e42(target: Pointer, color: Int): Pointer

    fun questpdf__text_block_descriptor__background_color__5461b453(target: Pointer, color: Int): Pointer

    fun questpdf__text_block_descriptor__font_size__c989487d(target: Pointer, value: Float): Pointer

    fun questpdf__text_block_descriptor__line_height__a1b4697a(target: Pointer, factor: Pointer): Pointer

    fun questpdf__text_block_descriptor__letter_spacing__92f86a26(target: Pointer, factor: Float): Pointer

    fun questpdf__text_block_descriptor__word_spacing__1f794add(target: Pointer, factor: Float): Pointer

    fun questpdf__text_block_descriptor__italic__4f023aba(target: Pointer, value: Byte): Pointer

    fun questpdf__text_block_descriptor__strikethrough__41841206(target: Pointer, value: Byte): Pointer

    fun questpdf__text_block_descriptor__underline__2e1ae473(target: Pointer, value: Byte): Pointer

    fun questpdf__text_block_descriptor__overline__add25860(target: Pointer, value: Byte): Pointer

    fun questpdf__text_block_descriptor__decoration_color__5d18d151(target: Pointer, color: Int): Pointer

    fun questpdf__text_block_descriptor__decoration_thickness__c7c23c84(target: Pointer, factor: Float): Pointer

    fun questpdf__text_block_descriptor__decoration_solid__f64746d1(target: Pointer): Pointer

    fun questpdf__text_block_descriptor__decoration_double__41cf8a18(target: Pointer): Pointer

    fun questpdf__text_block_descriptor__decoration_wavy__1761acf2(target: Pointer): Pointer

    fun questpdf__text_block_descriptor__decoration_dotted__e940537a(target: Pointer): Pointer

    fun questpdf__text_block_descriptor__decoration_dashed__a85f7344(target: Pointer): Pointer

    fun questpdf__text_block_descriptor__thin__e9036638(target: Pointer): Pointer

    fun questpdf__text_block_descriptor__extra_light__33bbe020(target: Pointer): Pointer

    fun questpdf__text_block_descriptor__light__37ef1bc2(target: Pointer): Pointer

    fun questpdf__text_block_descriptor__normal_weight__18d360b3(target: Pointer): Pointer

    fun questpdf__text_block_descriptor__medium__5ef8b80e(target: Pointer): Pointer

    fun questpdf__text_block_descriptor__semi_bold__0b92f7b7(target: Pointer): Pointer

    fun questpdf__text_block_descriptor__bold__0dfa9061(target: Pointer): Pointer

    fun questpdf__text_block_descriptor__extra_bold__c4fbc0a6(target: Pointer): Pointer

    fun questpdf__text_block_descriptor__black__0cc8d698(target: Pointer): Pointer

    fun questpdf__text_block_descriptor__extra_black__c7698d85(target: Pointer): Pointer

    fun questpdf__text_block_descriptor__normal_position__5e5176cb(target: Pointer): Pointer

    fun questpdf__text_block_descriptor__subscript__db9bd4eb(target: Pointer): Pointer

    fun questpdf__text_block_descriptor__superscript__a9b46b1e(target: Pointer): Pointer

    fun questpdf__text_block_descriptor__direction_auto__fbed9e71(target: Pointer): Pointer

    fun questpdf__text_block_descriptor__direction_from_left_to_right__09e2e3bc(target: Pointer): Pointer

    fun questpdf__text_block_descriptor__direction_from_right_to_left__6cc5cb9e(target: Pointer): Pointer

    fun questpdf__text_block_descriptor__enable_font_feature__136a164d(target: Pointer, featureName: String): Pointer

    fun questpdf__text_block_descriptor__disable_font_feature__5bd81de9(target: Pointer, featureName: String): Pointer



}

// ============================================================================
// TextBlockDescriptor Callback Interfaces
// ============================================================================


// ============================================================================
// TextBlockDescriptor Class
// ============================================================================

/**
 * TextBlockDescriptor wrapper for the native QuestPDF TextBlockDescriptor.
 */
class TextBlockDescriptor internal constructor(val pointer: Pointer) {
    private val callbacks = mutableListOf<Callback>()

    companion object {
        private var nativeLib: TextBlockDescriptorNative? = QuestLibrary.getLibrary(TextBlockDescriptorNative::class.java)
        private val initLock = Any()

        private fun lib(): TextBlockDescriptorNative {
            return nativeLib!!
        }

    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
    }
    

 


fun alignLeft(): TextBlockDescriptor {


    val result = lib().questpdf__text_block_descriptor__align_left__da139fee(pointer)
    return TextBlockDescriptor(result)

}
 

 


fun alignCenter(): TextBlockDescriptor {


    val result = lib().questpdf__text_block_descriptor__align_center__3661d942(pointer)
    return TextBlockDescriptor(result)

}
 

 


fun alignRight(): TextBlockDescriptor {


    val result = lib().questpdf__text_block_descriptor__align_right__28e79232(pointer)
    return TextBlockDescriptor(result)

}
 

 


fun justify(): TextBlockDescriptor {


    val result = lib().questpdf__text_block_descriptor__justify__f4a5d951(pointer)
    return TextBlockDescriptor(result)

}
 

 


fun alignStart(): TextBlockDescriptor {


    val result = lib().questpdf__text_block_descriptor__align_start__c97cfc2b(pointer)
    return TextBlockDescriptor(result)

}
 

 


fun alignEnd(): TextBlockDescriptor {


    val result = lib().questpdf__text_block_descriptor__align_end__e0ace0c1(pointer)
    return TextBlockDescriptor(result)

}
 

 


fun clampLines(maxLines: Int, ellipsis: String = "…"): TextBlockDescriptor {


    val result = lib().questpdf__text_block_descriptor__clamp_lines__2a5db05c(pointer, maxLines, ellipsis)
    return TextBlockDescriptor(result)

}
 

 


fun paragraphSpacing(value: Float, unit: LengthUnit = LengthUnit.Point): TextBlockDescriptor {


    val result = lib().questpdf__text_block_descriptor__paragraph_spacing__6dcddcea(pointer, value, unit.value)
    return TextBlockDescriptor(result)

}
 

 


fun paragraphFirstLineIndentation(value: Float, unit: LengthUnit = LengthUnit.Point): TextBlockDescriptor {


    val result = lib().questpdf__text_block_descriptor__paragraph_first_line_indentation__5d11cadd(pointer, value, unit.value)
    return TextBlockDescriptor(result)

}
 

 


fun fontColor(color: Color): TextBlockDescriptor {


    val result = lib().questpdf__text_block_descriptor__font_color__a0d06e42(pointer, color.hex.toInt())
    return TextBlockDescriptor(result)

}
 

 


fun backgroundColor(color: Color): TextBlockDescriptor {


    val result = lib().questpdf__text_block_descriptor__background_color__5461b453(pointer, color.hex.toInt())
    return TextBlockDescriptor(result)

}
 

 


fun fontSize(value: Float): TextBlockDescriptor {


    val result = lib().questpdf__text_block_descriptor__font_size__c989487d(pointer, value)
    return TextBlockDescriptor(result)

}
 

 


fun lineHeight(factor: Any): TextBlockDescriptor {


    val result = lib().questpdf__text_block_descriptor__line_height__a1b4697a(pointer, factor)
    return TextBlockDescriptor(result)

}
 

 


fun letterSpacing(factor: Float = 0f): TextBlockDescriptor {


    val result = lib().questpdf__text_block_descriptor__letter_spacing__92f86a26(pointer, factor)
    return TextBlockDescriptor(result)

}
 

 


fun wordSpacing(factor: Float = 0f): TextBlockDescriptor {


    val result = lib().questpdf__text_block_descriptor__word_spacing__1f794add(pointer, factor)
    return TextBlockDescriptor(result)

}
 

 


fun italic(value: Boolean = true): TextBlockDescriptor {


    val result = lib().questpdf__text_block_descriptor__italic__4f023aba(pointer, (if (value) 1.toByte() else 0.toByte()))
    return TextBlockDescriptor(result)

}
 

 


fun strikethrough(value: Boolean = true): TextBlockDescriptor {


    val result = lib().questpdf__text_block_descriptor__strikethrough__41841206(pointer, (if (value) 1.toByte() else 0.toByte()))
    return TextBlockDescriptor(result)

}
 

 


fun underline(value: Boolean = true): TextBlockDescriptor {


    val result = lib().questpdf__text_block_descriptor__underline__2e1ae473(pointer, (if (value) 1.toByte() else 0.toByte()))
    return TextBlockDescriptor(result)

}
 

 


fun overline(value: Boolean = true): TextBlockDescriptor {


    val result = lib().questpdf__text_block_descriptor__overline__add25860(pointer, (if (value) 1.toByte() else 0.toByte()))
    return TextBlockDescriptor(result)

}
 

 


fun decorationColor(color: Color): TextBlockDescriptor {


    val result = lib().questpdf__text_block_descriptor__decoration_color__5d18d151(pointer, color.hex.toInt())
    return TextBlockDescriptor(result)

}
 

 


fun decorationThickness(factor: Float): TextBlockDescriptor {


    val result = lib().questpdf__text_block_descriptor__decoration_thickness__c7c23c84(pointer, factor)
    return TextBlockDescriptor(result)

}
 

 


fun decorationSolid(): TextBlockDescriptor {


    val result = lib().questpdf__text_block_descriptor__decoration_solid__f64746d1(pointer)
    return TextBlockDescriptor(result)

}
 

 


fun decorationDouble(): TextBlockDescriptor {


    val result = lib().questpdf__text_block_descriptor__decoration_double__41cf8a18(pointer)
    return TextBlockDescriptor(result)

}
 

 


fun decorationWavy(): TextBlockDescriptor {


    val result = lib().questpdf__text_block_descriptor__decoration_wavy__1761acf2(pointer)
    return TextBlockDescriptor(result)

}
 

 


fun decorationDotted(): TextBlockDescriptor {


    val result = lib().questpdf__text_block_descriptor__decoration_dotted__e940537a(pointer)
    return TextBlockDescriptor(result)

}
 

 


fun decorationDashed(): TextBlockDescriptor {


    val result = lib().questpdf__text_block_descriptor__decoration_dashed__a85f7344(pointer)
    return TextBlockDescriptor(result)

}
 

 


fun thin(): TextBlockDescriptor {


    val result = lib().questpdf__text_block_descriptor__thin__e9036638(pointer)
    return TextBlockDescriptor(result)

}
 

 


fun extraLight(): TextBlockDescriptor {


    val result = lib().questpdf__text_block_descriptor__extra_light__33bbe020(pointer)
    return TextBlockDescriptor(result)

}
 

 


fun light(): TextBlockDescriptor {


    val result = lib().questpdf__text_block_descriptor__light__37ef1bc2(pointer)
    return TextBlockDescriptor(result)

}
 

 


fun normalWeight(): TextBlockDescriptor {


    val result = lib().questpdf__text_block_descriptor__normal_weight__18d360b3(pointer)
    return TextBlockDescriptor(result)

}
 

 


fun medium(): TextBlockDescriptor {


    val result = lib().questpdf__text_block_descriptor__medium__5ef8b80e(pointer)
    return TextBlockDescriptor(result)

}
 

 


fun semiBold(): TextBlockDescriptor {


    val result = lib().questpdf__text_block_descriptor__semi_bold__0b92f7b7(pointer)
    return TextBlockDescriptor(result)

}
 

 


fun bold(): TextBlockDescriptor {


    val result = lib().questpdf__text_block_descriptor__bold__0dfa9061(pointer)
    return TextBlockDescriptor(result)

}
 

 


fun extraBold(): TextBlockDescriptor {


    val result = lib().questpdf__text_block_descriptor__extra_bold__c4fbc0a6(pointer)
    return TextBlockDescriptor(result)

}
 

 


fun black(): TextBlockDescriptor {


    val result = lib().questpdf__text_block_descriptor__black__0cc8d698(pointer)
    return TextBlockDescriptor(result)

}
 

 


fun extraBlack(): TextBlockDescriptor {


    val result = lib().questpdf__text_block_descriptor__extra_black__c7698d85(pointer)
    return TextBlockDescriptor(result)

}
 

 


fun normalPosition(): TextBlockDescriptor {


    val result = lib().questpdf__text_block_descriptor__normal_position__5e5176cb(pointer)
    return TextBlockDescriptor(result)

}
 

 


fun subscript(): TextBlockDescriptor {


    val result = lib().questpdf__text_block_descriptor__subscript__db9bd4eb(pointer)
    return TextBlockDescriptor(result)

}
 

 


fun superscript(): TextBlockDescriptor {


    val result = lib().questpdf__text_block_descriptor__superscript__a9b46b1e(pointer)
    return TextBlockDescriptor(result)

}
 

 


fun directionAuto(): TextBlockDescriptor {


    val result = lib().questpdf__text_block_descriptor__direction_auto__fbed9e71(pointer)
    return TextBlockDescriptor(result)

}
 

 


fun directionFromLeftToRight(): TextBlockDescriptor {


    val result = lib().questpdf__text_block_descriptor__direction_from_left_to_right__09e2e3bc(pointer)
    return TextBlockDescriptor(result)

}
 

 


fun directionFromRightToLeft(): TextBlockDescriptor {


    val result = lib().questpdf__text_block_descriptor__direction_from_right_to_left__6cc5cb9e(pointer)
    return TextBlockDescriptor(result)

}
 

 


fun enableFontFeature(featureName: String): TextBlockDescriptor {


    val result = lib().questpdf__text_block_descriptor__enable_font_feature__136a164d(pointer, featureName)
    return TextBlockDescriptor(result)

}
 

 


fun disableFontFeature(featureName: String): TextBlockDescriptor {


    val result = lib().questpdf__text_block_descriptor__disable_font_feature__5bd81de9(pointer, featureName)
    return TextBlockDescriptor(result)

}
 


    
}




// ============================================================================
// ImageDescriptor - Native Function Interface
// ============================================================================



/**
 * JNA interface for ImageDescriptor native methods.
 */
private interface ImageDescriptorNative : Library {

    fun questpdf__image_descriptor__use_original_image__d3be84c2(target: Pointer, value: Byte): Pointer

    fun questpdf__image_descriptor__with_raster_dpi__78f617ee(target: Pointer, dpi: Int): Pointer

    fun questpdf__image_descriptor__with_compression_quality__1665a445(target: Pointer, quality: Int): Pointer

    fun questpdf__image_descriptor__fit_width__7b9aa4d6(target: Pointer): Pointer

    fun questpdf__image_descriptor__fit_height__c888daad(target: Pointer): Pointer

    fun questpdf__image_descriptor__fit_area__4dbf28f5(target: Pointer): Pointer

    fun questpdf__image_descriptor__fit_unproportionally__3d7bad76(target: Pointer): Pointer



}

// ============================================================================
// ImageDescriptor Callback Interfaces
// ============================================================================


// ============================================================================
// ImageDescriptor Class
// ============================================================================

/**
 * ImageDescriptor wrapper for the native QuestPDF ImageDescriptor.
 */
class ImageDescriptor internal constructor(val pointer: Pointer) {
    private val callbacks = mutableListOf<Callback>()

    companion object {
        private var nativeLib: ImageDescriptorNative? = QuestLibrary.getLibrary(ImageDescriptorNative::class.java)
        private val initLock = Any()

        private fun lib(): ImageDescriptorNative {
            return nativeLib!!
        }

    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
    }
    

 


fun useOriginalImage(value: Boolean = true): ImageDescriptor {


    val result = lib().questpdf__image_descriptor__use_original_image__d3be84c2(pointer, (if (value) 1.toByte() else 0.toByte()))
    return ImageDescriptor(result)

}
 

 


fun withRasterDpi(dpi: Int): ImageDescriptor {


    val result = lib().questpdf__image_descriptor__with_raster_dpi__78f617ee(pointer, dpi)
    return ImageDescriptor(result)

}
 

 


fun withCompressionQuality(quality: ImageCompressionQuality): ImageDescriptor {


    val result = lib().questpdf__image_descriptor__with_compression_quality__1665a445(pointer, quality.value)
    return ImageDescriptor(result)

}
 

 


fun fitWidth(): ImageDescriptor {


    val result = lib().questpdf__image_descriptor__fit_width__7b9aa4d6(pointer)
    return ImageDescriptor(result)

}
 

 


fun fitHeight(): ImageDescriptor {


    val result = lib().questpdf__image_descriptor__fit_height__c888daad(pointer)
    return ImageDescriptor(result)

}
 

 


fun fitArea(): ImageDescriptor {


    val result = lib().questpdf__image_descriptor__fit_area__4dbf28f5(pointer)
    return ImageDescriptor(result)

}
 

 


fun fitUnproportionally(): ImageDescriptor {


    val result = lib().questpdf__image_descriptor__fit_unproportionally__3d7bad76(pointer)
    return ImageDescriptor(result)

}
 


    
}




// ============================================================================
// DynamicImageDescriptor - Native Function Interface
// ============================================================================



/**
 * JNA interface for DynamicImageDescriptor native methods.
 */
private interface DynamicImageDescriptorNative : Library {

    fun questpdf__dynamic_image_descriptor__use_original_image__1dabc6b8(target: Pointer, value: Byte): Pointer

    fun questpdf__dynamic_image_descriptor__with_raster_dpi__a72018d5(target: Pointer, dpi: Int): Pointer

    fun questpdf__dynamic_image_descriptor__with_compression_quality__94465629(target: Pointer, quality: Int): Pointer



}

// ============================================================================
// DynamicImageDescriptor Callback Interfaces
// ============================================================================


// ============================================================================
// DynamicImageDescriptor Class
// ============================================================================

/**
 * DynamicImageDescriptor wrapper for the native QuestPDF DynamicImageDescriptor.
 */
class DynamicImageDescriptor internal constructor(val pointer: Pointer) {
    private val callbacks = mutableListOf<Callback>()

    companion object {
        private var nativeLib: DynamicImageDescriptorNative? = QuestLibrary.getLibrary(DynamicImageDescriptorNative::class.java)
        private val initLock = Any()

        private fun lib(): DynamicImageDescriptorNative {
            return nativeLib!!
        }

    
        
    
        
    
        
    
    }
    

 


fun useOriginalImage(value: Boolean = true): DynamicImageDescriptor {


    val result = lib().questpdf__dynamic_image_descriptor__use_original_image__1dabc6b8(pointer, (if (value) 1.toByte() else 0.toByte()))
    return DynamicImageDescriptor(result)

}
 

 


fun withRasterDpi(dpi: Int): DynamicImageDescriptor {


    val result = lib().questpdf__dynamic_image_descriptor__with_raster_dpi__a72018d5(pointer, dpi)
    return DynamicImageDescriptor(result)

}
 

 


fun withCompressionQuality(quality: ImageCompressionQuality): DynamicImageDescriptor {


    val result = lib().questpdf__dynamic_image_descriptor__with_compression_quality__94465629(pointer, quality.value)
    return DynamicImageDescriptor(result)

}
 


    
}




// ============================================================================
// SvgImageDescriptor - Native Function Interface
// ============================================================================



/**
 * JNA interface for SvgImageDescriptor native methods.
 */
private interface SvgImageDescriptorNative : Library {

    fun questpdf__svg_image_descriptor__fit_width__ae37e277(target: Pointer): Pointer

    fun questpdf__svg_image_descriptor__fit_height__7e11f169(target: Pointer): Pointer

    fun questpdf__svg_image_descriptor__fit_area__6abce393(target: Pointer): Pointer



}

// ============================================================================
// SvgImageDescriptor Callback Interfaces
// ============================================================================


// ============================================================================
// SvgImageDescriptor Class
// ============================================================================

/**
 * SvgImageDescriptor wrapper for the native QuestPDF SvgImageDescriptor.
 */
class SvgImageDescriptor internal constructor(val pointer: Pointer) {
    private val callbacks = mutableListOf<Callback>()

    companion object {
        private var nativeLib: SvgImageDescriptorNative? = QuestLibrary.getLibrary(SvgImageDescriptorNative::class.java)
        private val initLock = Any()

        private fun lib(): SvgImageDescriptorNative {
            return nativeLib!!
        }

    
        
    
        
    
        
    
    }
    

 


fun fitWidth(): SvgImageDescriptor {


    val result = lib().questpdf__svg_image_descriptor__fit_width__ae37e277(pointer)
    return SvgImageDescriptor(result)

}
 

 


fun fitHeight(): SvgImageDescriptor {


    val result = lib().questpdf__svg_image_descriptor__fit_height__7e11f169(pointer)
    return SvgImageDescriptor(result)

}
 

 


fun fitArea(): SvgImageDescriptor {


    val result = lib().questpdf__svg_image_descriptor__fit_area__6abce393(pointer)
    return SvgImageDescriptor(result)

}
 


    
}




// ============================================================================
// PageDescriptor - Native Function Interface
// ============================================================================



/**
 * JNA interface for PageDescriptor native methods.
 */
private interface PageDescriptorNative : Library {

    fun questpdf__page_descriptor__size__cd4bd97a(target: Pointer, width: Float, height: Float, unit: Int): Unit

    fun questpdf__page_descriptor__size__593af4d7(target: Pointer, pageSize: Pointer): Unit

    fun questpdf__page_descriptor__continuous_size__ae1c9536(target: Pointer, width: Float, unit: Int): Unit

    fun questpdf__page_descriptor__min_size__8216ba5f(target: Pointer, pageSize: Pointer): Unit

    fun questpdf__page_descriptor__max_size__02a75d07(target: Pointer, pageSize: Pointer): Unit

    fun questpdf__page_descriptor__margin_left__4c6b8a4c(target: Pointer, value: Float, unit: Int): Unit

    fun questpdf__page_descriptor__margin_right__de714424(target: Pointer, value: Float, unit: Int): Unit

    fun questpdf__page_descriptor__margin_top__51f73aad(target: Pointer, value: Float, unit: Int): Unit

    fun questpdf__page_descriptor__margin_bottom__d37c07b0(target: Pointer, value: Float, unit: Int): Unit

    fun questpdf__page_descriptor__margin_vertical__97f4d868(target: Pointer, value: Float, unit: Int): Unit

    fun questpdf__page_descriptor__margin_horizontal__bafb50fd(target: Pointer, value: Float, unit: Int): Unit

    fun questpdf__page_descriptor__margin__35d8c8b5(target: Pointer, value: Float, unit: Int): Unit

    fun questpdf__page_descriptor__content_from_left_to_right__6bcc64b0(target: Pointer): Unit

    fun questpdf__page_descriptor__content_from_right_to_left__dbce6933(target: Pointer): Unit

    fun questpdf__page_descriptor__page_color__7ab89cfe(target: Pointer, color: Int): Unit

    fun questpdf__page_descriptor__background__8048fed3(target: Pointer): Pointer

    fun questpdf__page_descriptor__foreground__b6d36c4d(target: Pointer): Pointer

    fun questpdf__page_descriptor__header__392dd2be(target: Pointer): Pointer

    fun questpdf__page_descriptor__content__af503598(target: Pointer): Pointer

    fun questpdf__page_descriptor__footer__eb07832c(target: Pointer): Pointer



}

// ============================================================================
// PageDescriptor Callback Interfaces
// ============================================================================


// ============================================================================
// PageDescriptor Class
// ============================================================================

/**
 * PageDescriptor wrapper for the native QuestPDF PageDescriptor.
 */
class PageDescriptor internal constructor(val pointer: Pointer) {
    private val callbacks = mutableListOf<Callback>()

    companion object {
        private var nativeLib: PageDescriptorNative? = QuestLibrary.getLibrary(PageDescriptorNative::class.java)
        private val initLock = Any()

        private fun lib(): PageDescriptorNative {
            return nativeLib!!
        }

    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
    }
    

 


fun size(width: Float, height: Float, unit: LengthUnit = LengthUnit.Point) {


    lib().questpdf__page_descriptor__size__cd4bd97a(pointer, width, height, unit.value)

}
 

 


fun size(pageSize: PageSize) {


    lib().questpdf__page_descriptor__size__593af4d7(pointer, pageSize.pointer)

}
 

 


fun continuousSize(width: Float, unit: LengthUnit = LengthUnit.Point) {


    lib().questpdf__page_descriptor__continuous_size__ae1c9536(pointer, width, unit.value)

}
 

 


fun minSize(pageSize: PageSize) {


    lib().questpdf__page_descriptor__min_size__8216ba5f(pointer, pageSize.pointer)

}
 

 


fun maxSize(pageSize: PageSize) {


    lib().questpdf__page_descriptor__max_size__02a75d07(pointer, pageSize.pointer)

}
 

 


fun marginLeft(value: Float, unit: LengthUnit = LengthUnit.Point) {


    lib().questpdf__page_descriptor__margin_left__4c6b8a4c(pointer, value, unit.value)

}
 

 


fun marginRight(value: Float, unit: LengthUnit = LengthUnit.Point) {


    lib().questpdf__page_descriptor__margin_right__de714424(pointer, value, unit.value)

}
 

 


fun marginTop(value: Float, unit: LengthUnit = LengthUnit.Point) {


    lib().questpdf__page_descriptor__margin_top__51f73aad(pointer, value, unit.value)

}
 

 


fun marginBottom(value: Float, unit: LengthUnit = LengthUnit.Point) {


    lib().questpdf__page_descriptor__margin_bottom__d37c07b0(pointer, value, unit.value)

}
 

 


fun marginVertical(value: Float, unit: LengthUnit = LengthUnit.Point) {


    lib().questpdf__page_descriptor__margin_vertical__97f4d868(pointer, value, unit.value)

}
 

 


fun marginHorizontal(value: Float, unit: LengthUnit = LengthUnit.Point) {


    lib().questpdf__page_descriptor__margin_horizontal__bafb50fd(pointer, value, unit.value)

}
 

 


fun margin(value: Float, unit: LengthUnit = LengthUnit.Point) {


    lib().questpdf__page_descriptor__margin__35d8c8b5(pointer, value, unit.value)

}
 

 


fun contentFromLeftToRight() {


    lib().questpdf__page_descriptor__content_from_left_to_right__6bcc64b0(pointer)

}
 

 


fun contentFromRightToLeft() {


    lib().questpdf__page_descriptor__content_from_right_to_left__dbce6933(pointer)

}
 

 


fun pageColor(color: Color) {


    lib().questpdf__page_descriptor__page_color__7ab89cfe(pointer, color.hex.toInt())

}
 

 


fun background(): Container {


    val result = lib().questpdf__page_descriptor__background__8048fed3(pointer)
    return Container(result)

}
 

 


fun foreground(): Container {


    val result = lib().questpdf__page_descriptor__foreground__b6d36c4d(pointer)
    return Container(result)

}
 

 


fun header(): Container {


    val result = lib().questpdf__page_descriptor__header__392dd2be(pointer)
    return Container(result)

}
 

 


fun content(): Container {


    val result = lib().questpdf__page_descriptor__content__af503598(pointer)
    return Container(result)

}
 

 


fun footer(): Container {


    val result = lib().questpdf__page_descriptor__footer__eb07832c(pointer)
    return Container(result)

}
 


    
}




// ============================================================================
// Container - Native Function Interface
// ============================================================================



/**
 * JNA interface for Container native methods.
 */
private interface ContainerNative : Library {

    fun questpdf__container__align_left__68bfdc67(target: Pointer): Pointer

    fun questpdf__container__align_center__4fb1e0d1(target: Pointer): Pointer

    fun questpdf__container__align_right__a1c1a1bf(target: Pointer): Pointer

    fun questpdf__container__align_top__f275ca95(target: Pointer): Pointer

    fun questpdf__container__align_middle__95fef9e8(target: Pointer): Pointer

    fun questpdf__container__align_bottom__d33d0520(target: Pointer): Pointer

    fun questpdf__container__column__24d6ceed(target: Pointer, handler: Callback): Unit

    fun questpdf__container__width__a346e20f(target: Pointer, value: Float, unit: Int): Pointer

    fun questpdf__container__min_width__c00f1915(target: Pointer, value: Float, unit: Int): Pointer

    fun questpdf__container__max_width__7e85a057(target: Pointer, value: Float, unit: Int): Pointer

    fun questpdf__container__height__36ac3a02(target: Pointer, value: Float, unit: Int): Pointer

    fun questpdf__container__min_height__58cc06b0(target: Pointer, value: Float, unit: Int): Pointer

    fun questpdf__container__max_height__0b76e199(target: Pointer, value: Float, unit: Int): Pointer

    fun questpdf__container__content_from_left_to_right__191523c1(target: Pointer): Pointer

    fun questpdf__container__content_from_right_to_left__a31dbd9d(target: Pointer): Pointer

    fun questpdf__container__debug_area__a69b9c65(target: Pointer, text: String, color: Int): Pointer

    fun questpdf__container__debug_pointer__9d669879(target: Pointer, label: String): Pointer

    fun questpdf__container__decoration__0b39c58e(target: Pointer, handler: Callback): Unit

    fun questpdf__container__aspect_ratio__fd5bc0dc(target: Pointer, ratio: Float, option: Int): Pointer

    fun questpdf__container__placeholder__a560192f(target: Pointer, text: String): Unit

    fun questpdf__container__show_once__c6224013(target: Pointer): Pointer

    fun questpdf__container__skip_once__b3d4c7bf(target: Pointer): Pointer

    fun questpdf__container__show_entire__16629c88(target: Pointer): Pointer

    fun questpdf__container__ensure_space__0cbedd6a(target: Pointer, minHeight: Float): Pointer

    fun questpdf__container__prevent_page_break__2e3cab6a(target: Pointer): Pointer

    fun questpdf__container__page_break__4287fb55(target: Pointer): Unit

    fun questpdf__container__container__be126adc(target: Pointer): Pointer

    fun questpdf__container__hyperlink__40aee55c(target: Pointer, url: String): Pointer

    fun questpdf__container__section__b2687119(target: Pointer, sectionName: String): Pointer

    fun questpdf__container__section_link__d27b4828(target: Pointer, sectionName: String): Pointer

    fun questpdf__container__show_if__da52e306(target: Pointer, condition: Byte): Pointer

    fun questpdf__container__unconstrained__a43107f6(target: Pointer): Pointer

    fun questpdf__container__stop_paging__81b05f34(target: Pointer): Pointer

    fun questpdf__container__scale_to_fit__bb0b4e57(target: Pointer): Pointer

    fun questpdf__container__repeat__e198bc84(target: Pointer): Pointer

    fun questpdf__container__lazy__971e7b54(target: Pointer, contentBuilder: Callback): Unit

    fun questpdf__container__lazy_with_cache__a33b5f9b(target: Pointer, contentBuilder: Callback): Unit

    fun questpdf__container__z_index__9cd9a32e(target: Pointer, indexValue: Int): Pointer

    fun questpdf__container__capture_content_position__845fb313(target: Pointer, id: String): Pointer

    fun questpdf__container__extend__291e835a(target: Pointer): Pointer

    fun questpdf__container__extend_vertical__e63e1d72(target: Pointer): Pointer

    fun questpdf__container__extend_horizontal__c6d6d128(target: Pointer): Pointer

    fun questpdf__container__image__9155d14a(target: Pointer, filePath: String): Pointer

    fun questpdf__container__image__ccf976d1(target: Pointer, image: Pointer): Pointer

    fun questpdf__container__inlined__33b27c8d(target: Pointer, handler: Callback): Unit

    fun questpdf__container__layers__03ce5bdd(target: Pointer, handler: Callback): Unit

    fun questpdf__container__line_vertical__ab97b857(target: Pointer, thickness: Float, unit: Int): Pointer

    fun questpdf__container__line_horizontal__a6f7f11f(target: Pointer, thickness: Float, unit: Int): Pointer

    fun questpdf__container__multi_column__193479d6(target: Pointer, handler: Callback): Unit

    fun questpdf__container__padding__5daecb7e(target: Pointer, value: Float, unit: Int): Pointer

    fun questpdf__container__padding_horizontal__7a6b255d(target: Pointer, value: Float, unit: Int): Pointer

    fun questpdf__container__padding_vertical__91122aaa(target: Pointer, value: Float, unit: Int): Pointer

    fun questpdf__container__padding_top__de3b7b3b(target: Pointer, value: Float, unit: Int): Pointer

    fun questpdf__container__padding_bottom__74ad0a7b(target: Pointer, value: Float, unit: Int): Pointer

    fun questpdf__container__padding_left__103ee738(target: Pointer, value: Float, unit: Int): Pointer

    fun questpdf__container__padding_right__89d1cf61(target: Pointer, value: Float, unit: Int): Pointer

    fun questpdf__container__rotate_left__c5193e66(target: Pointer): Pointer

    fun questpdf__container__rotate_right__004c9c52(target: Pointer): Pointer

    fun questpdf__container__rotate__c33f62ac(target: Pointer, angle: Float): Pointer

    fun questpdf__container__row__39fce557(target: Pointer, handler: Callback): Unit

    fun questpdf__container__scale__05521931(target: Pointer, factor: Float): Pointer

    fun questpdf__container__scale_horizontal__14d1a9be(target: Pointer, factor: Float): Pointer

    fun questpdf__container__scale_vertical__5bc8a8a5(target: Pointer, factor: Float): Pointer

    fun questpdf__container__flip_horizontal__744e4fe9(target: Pointer): Pointer

    fun questpdf__container__flip_vertical__a91487f3(target: Pointer): Pointer

    fun questpdf__container__flip_over__ce1ff345(target: Pointer): Pointer

    fun questpdf__container__shrink__4221b85b(target: Pointer): Pointer

    fun questpdf__container__shrink_vertical__e5042c3c(target: Pointer): Pointer

    fun questpdf__container__shrink_horizontal__588cfd0f(target: Pointer): Pointer

    fun questpdf__container__border__a6712928(target: Pointer, all: Float, color: Int): Pointer

    fun questpdf__container__background__68f98b81(target: Pointer, color: Int): Pointer

    fun questpdf__container__border__17f3b5e4(target: Pointer, value: Float, unit: Int): Pointer

    fun questpdf__container__border_vertical__7922384b(target: Pointer, value: Float, unit: Int): Pointer

    fun questpdf__container__border_horizontal__34913f34(target: Pointer, value: Float, unit: Int): Pointer

    fun questpdf__container__border_left__803ed1e6(target: Pointer, value: Float, unit: Int): Pointer

    fun questpdf__container__border_right__de8ca6bf(target: Pointer, value: Float, unit: Int): Pointer

    fun questpdf__container__border_top__c469b91f(target: Pointer, value: Float, unit: Int): Pointer

    fun questpdf__container__border_bottom__59b8a019(target: Pointer, value: Float, unit: Int): Pointer

    fun questpdf__container__corner_radius__bf7cb39f(target: Pointer, value: Float, unit: Int): Pointer

    fun questpdf__container__corner_radius_top_left__41d08c72(target: Pointer, value: Float, unit: Int): Pointer

    fun questpdf__container__corner_radius_top_right__1497678a(target: Pointer, value: Float, unit: Int): Pointer

    fun questpdf__container__corner_radius_bottom_left__3a8d234a(target: Pointer, value: Float, unit: Int): Pointer

    fun questpdf__container__corner_radius_bottom_right__b07c1d8d(target: Pointer, value: Float, unit: Int): Pointer

    fun questpdf__container__border_color__2a24bda0(target: Pointer, color: Int): Pointer

    fun questpdf__container__border_alignment_outside__ce5e63fa(target: Pointer): Pointer

    fun questpdf__container__border_alignment_middle__66a27445(target: Pointer): Pointer

    fun questpdf__container__border_alignment_inside__8cef56b1(target: Pointer): Pointer

    fun questpdf__container__svg__f547d46e(target: Pointer, svg: String): Pointer

    fun questpdf__container__svg__b1de06e3(target: Pointer, image: Pointer): Pointer

    fun questpdf__container__table__d49da987(target: Pointer, handler: Callback): Unit

    fun questpdf__container__text__357e362f(target: Pointer, content: Callback): Unit

    fun questpdf__container__text__3f6b5b07(target: Pointer, text: String): Pointer

    fun questpdf__container__translate_x__351baebe(target: Pointer, value: Float, unit: Int): Pointer

    fun questpdf__container__translate_y__d99602db(target: Pointer, value: Float, unit: Int): Pointer


fun questpdf__container__background_linear_gradient(target: Pointer, angle: Float, colors: IntArray, colorsLength: Int): Pointer
fun questpdf__container__border_linear_gradient(target: Pointer, angle: Float, colors: IntArray, colorsLength: Int): Pointer
fun questpdf__container__shadow(target: Pointer, blur: Float, color: Int, offsetX: Float, offset: Float, spread: Float): Pointer
}

// ============================================================================
// Container Callback Interfaces
// ============================================================================


/**
 * Callback interface for ColumnDescriptor configuration.
 */
interface ColumnDescriptorCallback : Callback {
    fun callback(ptr: Pointer)
}


/**
 * Callback interface for DecorationDescriptor configuration.
 */
interface DecorationDescriptorCallback : Callback {
    fun callback(ptr: Pointer)
}


/**
 * Callback interface for Container configuration.
 */
interface ContainerCallback : Callback {
    fun callback(ptr: Pointer)
}


/**
 * Callback interface for InlinedDescriptor configuration.
 */
interface InlinedDescriptorCallback : Callback {
    fun callback(ptr: Pointer)
}


/**
 * Callback interface for LayersDescriptor configuration.
 */
interface LayersDescriptorCallback : Callback {
    fun callback(ptr: Pointer)
}


/**
 * Callback interface for MultiColumnDescriptor configuration.
 */
interface MultiColumnDescriptorCallback : Callback {
    fun callback(ptr: Pointer)
}


/**
 * Callback interface for RowDescriptor configuration.
 */
interface RowDescriptorCallback : Callback {
    fun callback(ptr: Pointer)
}


/**
 * Callback interface for TableDescriptor configuration.
 */
interface TableDescriptorCallback : Callback {
    fun callback(ptr: Pointer)
}


/**
 * Callback interface for TextDescriptor configuration.
 */
interface TextDescriptorCallback : Callback {
    fun callback(ptr: Pointer)
}


// ============================================================================
// Container Class
// ============================================================================

/**
 * Container wrapper for the native QuestPDF Container.
 */
class Container internal constructor(val pointer: Pointer) {
    private val callbacks = mutableListOf<Callback>()

    companion object {
        private var nativeLib: ContainerNative? = QuestLibrary.getLibrary(ContainerNative::class.java)
        private val initLock = Any()

        private fun lib(): ContainerNative {
            return nativeLib!!
        }

    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
        
    
    }
    

 


fun alignLeft(): Container {


    val result = lib().questpdf__container__align_left__68bfdc67(pointer)
    return Container(result)

}
 

 


fun alignCenter(): Container {


    val result = lib().questpdf__container__align_center__4fb1e0d1(pointer)
    return Container(result)

}
 

 


fun alignRight(): Container {


    val result = lib().questpdf__container__align_right__a1c1a1bf(pointer)
    return Container(result)

}
 

 


fun alignTop(): Container {


    val result = lib().questpdf__container__align_top__f275ca95(pointer)
    return Container(result)

}
 

 


fun alignMiddle(): Container {


    val result = lib().questpdf__container__align_middle__95fef9e8(pointer)
    return Container(result)

}
 

 


fun alignBottom(): Container {


    val result = lib().questpdf__container__align_bottom__d33d0520(pointer)
    return Container(result)

}
 

 


fun column(handler: (ColumnDescriptor) -> Unit) {

    // Create callback for handler
    val handlerCallback = object : ColumnDescriptorCallback {
    override fun callback(ptr: Pointer) {
    val obj = ColumnDescriptor(ptr)
    handler(obj)
    }
    }
    callbacks.add(handlerCallback)



    lib().questpdf__container__column__24d6ceed(pointer, handlerCallback)

}
 

 


fun width(value: Float, unit: LengthUnit = LengthUnit.Point): Container {


    val result = lib().questpdf__container__width__a346e20f(pointer, value, unit.value)
    return Container(result)

}
 

 


fun minWidth(value: Float, unit: LengthUnit = LengthUnit.Point): Container {


    val result = lib().questpdf__container__min_width__c00f1915(pointer, value, unit.value)
    return Container(result)

}
 

 


fun maxWidth(value: Float, unit: LengthUnit = LengthUnit.Point): Container {


    val result = lib().questpdf__container__max_width__7e85a057(pointer, value, unit.value)
    return Container(result)

}
 

 


fun height(value: Float, unit: LengthUnit = LengthUnit.Point): Container {


    val result = lib().questpdf__container__height__36ac3a02(pointer, value, unit.value)
    return Container(result)

}
 

 


fun minHeight(value: Float, unit: LengthUnit = LengthUnit.Point): Container {


    val result = lib().questpdf__container__min_height__58cc06b0(pointer, value, unit.value)
    return Container(result)

}
 

 


fun maxHeight(value: Float, unit: LengthUnit = LengthUnit.Point): Container {


    val result = lib().questpdf__container__max_height__0b76e199(pointer, value, unit.value)
    return Container(result)

}
 

 


fun contentFromLeftToRight(): Container {


    val result = lib().questpdf__container__content_from_left_to_right__191523c1(pointer)
    return Container(result)

}
 

 


fun contentFromRightToLeft(): Container {


    val result = lib().questpdf__container__content_from_right_to_left__a31dbd9d(pointer)
    return Container(result)

}
 

 


fun debugArea(text: String = null, color: Color = null): Container {


    val result = lib().questpdf__container__debug_area__a69b9c65(pointer, text, color.hex.toInt())
    return Container(result)

}
 

 


fun debugPointer(label: String): Container {


    val result = lib().questpdf__container__debug_pointer__9d669879(pointer, label)
    return Container(result)

}
 

 


fun decoration(handler: (DecorationDescriptor) -> Unit) {

    // Create callback for handler
    val handlerCallback = object : DecorationDescriptorCallback {
    override fun callback(ptr: Pointer) {
    val obj = DecorationDescriptor(ptr)
    handler(obj)
    }
    }
    callbacks.add(handlerCallback)



    lib().questpdf__container__decoration__0b39c58e(pointer, handlerCallback)

}
 

 


fun aspectRatio(ratio: Float, option: AspectRatioOption = AspectRatioOption.FitWidth): Container {


    val result = lib().questpdf__container__aspect_ratio__fd5bc0dc(pointer, ratio, option.value)
    return Container(result)

}
 

 


fun placeholder(text: String = null) {


    lib().questpdf__container__placeholder__a560192f(pointer, text)

}
 

 


fun showOnce(): Container {


    val result = lib().questpdf__container__show_once__c6224013(pointer)
    return Container(result)

}
 

 


fun skipOnce(): Container {


    val result = lib().questpdf__container__skip_once__b3d4c7bf(pointer)
    return Container(result)

}
 

 


fun showEntire(): Container {


    val result = lib().questpdf__container__show_entire__16629c88(pointer)
    return Container(result)

}
 

 


fun ensureSpace(minHeight: Float = 150f): Container {


    val result = lib().questpdf__container__ensure_space__0cbedd6a(pointer, minHeight)
    return Container(result)

}
 

 


fun preventPageBreak(): Container {


    val result = lib().questpdf__container__prevent_page_break__2e3cab6a(pointer)
    return Container(result)

}
 

 


fun pageBreak() {


    lib().questpdf__container__page_break__4287fb55(pointer)

}
 

 


fun container(): Container {


    val result = lib().questpdf__container__container__be126adc(pointer)
    return Container(result)

}
 

 


fun hyperlink(url: String): Container {


    val result = lib().questpdf__container__hyperlink__40aee55c(pointer, url)
    return Container(result)

}
 

 


fun section(sectionName: String): Container {


    val result = lib().questpdf__container__section__b2687119(pointer, sectionName)
    return Container(result)

}
 

 


fun sectionLink(sectionName: String): Container {


    val result = lib().questpdf__container__section_link__d27b4828(pointer, sectionName)
    return Container(result)

}
 

 


fun showIf(condition: Boolean): Container {


    val result = lib().questpdf__container__show_if__da52e306(pointer, (if (condition) 1.toByte() else 0.toByte()))
    return Container(result)

}
 

 


fun unconstrained(): Container {


    val result = lib().questpdf__container__unconstrained__a43107f6(pointer)
    return Container(result)

}
 

 


fun stopPaging(): Container {


    val result = lib().questpdf__container__stop_paging__81b05f34(pointer)
    return Container(result)

}
 

 


fun scaleToFit(): Container {


    val result = lib().questpdf__container__scale_to_fit__bb0b4e57(pointer)
    return Container(result)

}
 

 


fun repeat(): Container {


    val result = lib().questpdf__container__repeat__e198bc84(pointer)
    return Container(result)

}
 

 


fun lazy(contentBuilder: (Container) -> Unit) {

    // Create callback for contentBuilder
    val contentBuilderCallback = object : ContainerCallback {
    override fun callback(ptr: Pointer) {
    val obj = Container(ptr)
    contentBuilder(obj)
    }
    }
    callbacks.add(contentBuilderCallback)



    lib().questpdf__container__lazy__971e7b54(pointer, contentBuilderCallback)

}
 

 


fun lazyWithCache(contentBuilder: (Container) -> Unit) {

    // Create callback for contentBuilder
    val contentBuilderCallback = object : ContainerCallback {
    override fun callback(ptr: Pointer) {
    val obj = Container(ptr)
    contentBuilder(obj)
    }
    }
    callbacks.add(contentBuilderCallback)



    lib().questpdf__container__lazy_with_cache__a33b5f9b(pointer, contentBuilderCallback)

}
 

 


fun zIndex(indexValue: Int): Container {


    val result = lib().questpdf__container__z_index__9cd9a32e(pointer, indexValue)
    return Container(result)

}
 

 


fun captureContentPosition(id: String): Container {


    val result = lib().questpdf__container__capture_content_position__845fb313(pointer, id)
    return Container(result)

}
 

 


fun extend(): Container {


    val result = lib().questpdf__container__extend__291e835a(pointer)
    return Container(result)

}
 

 


fun extendVertical(): Container {


    val result = lib().questpdf__container__extend_vertical__e63e1d72(pointer)
    return Container(result)

}
 

 


fun extendHorizontal(): Container {


    val result = lib().questpdf__container__extend_horizontal__c6d6d128(pointer)
    return Container(result)

}
 

 


fun image(filePath: String): ImageDescriptor {


    val result = lib().questpdf__container__image__9155d14a(pointer, filePath)
    return ImageDescriptor(result)

}
 

 


fun image(image: Image): ImageDescriptor {


    val result = lib().questpdf__container__image__ccf976d1(pointer, image.pointer)
    return ImageDescriptor(result)

}
 

 


fun inlined(handler: (InlinedDescriptor) -> Unit) {

    // Create callback for handler
    val handlerCallback = object : InlinedDescriptorCallback {
    override fun callback(ptr: Pointer) {
    val obj = InlinedDescriptor(ptr)
    handler(obj)
    }
    }
    callbacks.add(handlerCallback)



    lib().questpdf__container__inlined__33b27c8d(pointer, handlerCallback)

}
 

 


fun layers(handler: (LayersDescriptor) -> Unit) {

    // Create callback for handler
    val handlerCallback = object : LayersDescriptorCallback {
    override fun callback(ptr: Pointer) {
    val obj = LayersDescriptor(ptr)
    handler(obj)
    }
    }
    callbacks.add(handlerCallback)



    lib().questpdf__container__layers__03ce5bdd(pointer, handlerCallback)

}
 

 


fun lineVertical(thickness: Float, unit: LengthUnit = LengthUnit.Point): LineDescriptor {


    val result = lib().questpdf__container__line_vertical__ab97b857(pointer, thickness, unit.value)
    return LineDescriptor(result)

}
 

 


fun lineHorizontal(thickness: Float, unit: LengthUnit = LengthUnit.Point): LineDescriptor {


    val result = lib().questpdf__container__line_horizontal__a6f7f11f(pointer, thickness, unit.value)
    return LineDescriptor(result)

}
 

 


fun multiColumn(handler: (MultiColumnDescriptor) -> Unit) {

    // Create callback for handler
    val handlerCallback = object : MultiColumnDescriptorCallback {
    override fun callback(ptr: Pointer) {
    val obj = MultiColumnDescriptor(ptr)
    handler(obj)
    }
    }
    callbacks.add(handlerCallback)



    lib().questpdf__container__multi_column__193479d6(pointer, handlerCallback)

}
 

 


fun padding(value: Float, unit: LengthUnit = LengthUnit.Point): Container {


    val result = lib().questpdf__container__padding__5daecb7e(pointer, value, unit.value)
    return Container(result)

}
 

 


fun paddingHorizontal(value: Float, unit: LengthUnit = LengthUnit.Point): Container {


    val result = lib().questpdf__container__padding_horizontal__7a6b255d(pointer, value, unit.value)
    return Container(result)

}
 

 


fun paddingVertical(value: Float, unit: LengthUnit = LengthUnit.Point): Container {


    val result = lib().questpdf__container__padding_vertical__91122aaa(pointer, value, unit.value)
    return Container(result)

}
 

 


fun paddingTop(value: Float, unit: LengthUnit = LengthUnit.Point): Container {


    val result = lib().questpdf__container__padding_top__de3b7b3b(pointer, value, unit.value)
    return Container(result)

}
 

 


fun paddingBottom(value: Float, unit: LengthUnit = LengthUnit.Point): Container {


    val result = lib().questpdf__container__padding_bottom__74ad0a7b(pointer, value, unit.value)
    return Container(result)

}
 

 


fun paddingLeft(value: Float, unit: LengthUnit = LengthUnit.Point): Container {


    val result = lib().questpdf__container__padding_left__103ee738(pointer, value, unit.value)
    return Container(result)

}
 

 


fun paddingRight(value: Float, unit: LengthUnit = LengthUnit.Point): Container {


    val result = lib().questpdf__container__padding_right__89d1cf61(pointer, value, unit.value)
    return Container(result)

}
 

 


fun rotateLeft(): Container {


    val result = lib().questpdf__container__rotate_left__c5193e66(pointer)
    return Container(result)

}
 

 


fun rotateRight(): Container {


    val result = lib().questpdf__container__rotate_right__004c9c52(pointer)
    return Container(result)

}
 

 


fun rotate(angle: Float): Container {


    val result = lib().questpdf__container__rotate__c33f62ac(pointer, angle)
    return Container(result)

}
 

 


fun row(handler: (RowDescriptor) -> Unit) {

    // Create callback for handler
    val handlerCallback = object : RowDescriptorCallback {
    override fun callback(ptr: Pointer) {
    val obj = RowDescriptor(ptr)
    handler(obj)
    }
    }
    callbacks.add(handlerCallback)



    lib().questpdf__container__row__39fce557(pointer, handlerCallback)

}
 

 


fun scale(factor: Float): Container {


    val result = lib().questpdf__container__scale__05521931(pointer, factor)
    return Container(result)

}
 

 


fun scaleHorizontal(factor: Float): Container {


    val result = lib().questpdf__container__scale_horizontal__14d1a9be(pointer, factor)
    return Container(result)

}
 

 


fun scaleVertical(factor: Float): Container {


    val result = lib().questpdf__container__scale_vertical__5bc8a8a5(pointer, factor)
    return Container(result)

}
 

 


fun flipHorizontal(): Container {


    val result = lib().questpdf__container__flip_horizontal__744e4fe9(pointer)
    return Container(result)

}
 

 


fun flipVertical(): Container {


    val result = lib().questpdf__container__flip_vertical__a91487f3(pointer)
    return Container(result)

}
 

 


fun flipOver(): Container {


    val result = lib().questpdf__container__flip_over__ce1ff345(pointer)
    return Container(result)

}
 

 


fun shrink(): Container {


    val result = lib().questpdf__container__shrink__4221b85b(pointer)
    return Container(result)

}
 

 


fun shrinkVertical(): Container {


    val result = lib().questpdf__container__shrink_vertical__e5042c3c(pointer)
    return Container(result)

}
 

 


fun shrinkHorizontal(): Container {


    val result = lib().questpdf__container__shrink_horizontal__588cfd0f(pointer)
    return Container(result)

}
 

 


fun border(all: Float, color: Color): Container {


    val result = lib().questpdf__container__border__a6712928(pointer, all, color.hex.toInt())
    return Container(result)

}
 

 


fun background(color: Color): Container {


    val result = lib().questpdf__container__background__68f98b81(pointer, color.hex.toInt())
    return Container(result)

}
 

 


fun border(value: Float, unit: LengthUnit = LengthUnit.Point): Container {


    val result = lib().questpdf__container__border__17f3b5e4(pointer, value, unit.value)
    return Container(result)

}
 

 


fun borderVertical(value: Float, unit: LengthUnit = LengthUnit.Point): Container {


    val result = lib().questpdf__container__border_vertical__7922384b(pointer, value, unit.value)
    return Container(result)

}
 

 


fun borderHorizontal(value: Float, unit: LengthUnit = LengthUnit.Point): Container {


    val result = lib().questpdf__container__border_horizontal__34913f34(pointer, value, unit.value)
    return Container(result)

}
 

 


fun borderLeft(value: Float, unit: LengthUnit = LengthUnit.Point): Container {


    val result = lib().questpdf__container__border_left__803ed1e6(pointer, value, unit.value)
    return Container(result)

}
 

 


fun borderRight(value: Float, unit: LengthUnit = LengthUnit.Point): Container {


    val result = lib().questpdf__container__border_right__de8ca6bf(pointer, value, unit.value)
    return Container(result)

}
 

 


fun borderTop(value: Float, unit: LengthUnit = LengthUnit.Point): Container {


    val result = lib().questpdf__container__border_top__c469b91f(pointer, value, unit.value)
    return Container(result)

}
 

 


fun borderBottom(value: Float, unit: LengthUnit = LengthUnit.Point): Container {


    val result = lib().questpdf__container__border_bottom__59b8a019(pointer, value, unit.value)
    return Container(result)

}
 

 


fun cornerRadius(value: Float = 0f, unit: LengthUnit = LengthUnit.Point): Container {


    val result = lib().questpdf__container__corner_radius__bf7cb39f(pointer, value, unit.value)
    return Container(result)

}
 

 


fun cornerRadiusTopLeft(value: Float = 0f, unit: LengthUnit = LengthUnit.Point): Container {


    val result = lib().questpdf__container__corner_radius_top_left__41d08c72(pointer, value, unit.value)
    return Container(result)

}
 

 


fun cornerRadiusTopRight(value: Float = 0f, unit: LengthUnit = LengthUnit.Point): Container {


    val result = lib().questpdf__container__corner_radius_top_right__1497678a(pointer, value, unit.value)
    return Container(result)

}
 

 


fun cornerRadiusBottomLeft(value: Float = 0f, unit: LengthUnit = LengthUnit.Point): Container {


    val result = lib().questpdf__container__corner_radius_bottom_left__3a8d234a(pointer, value, unit.value)
    return Container(result)

}
 

 


fun cornerRadiusBottomRight(value: Float = 0f, unit: LengthUnit = LengthUnit.Point): Container {


    val result = lib().questpdf__container__corner_radius_bottom_right__b07c1d8d(pointer, value, unit.value)
    return Container(result)

}
 

 


fun borderColor(color: Color): Container {


    val result = lib().questpdf__container__border_color__2a24bda0(pointer, color.hex.toInt())
    return Container(result)

}
 

 


fun borderAlignmentOutside(): Container {


    val result = lib().questpdf__container__border_alignment_outside__ce5e63fa(pointer)
    return Container(result)

}
 

 


fun borderAlignmentMiddle(): Container {


    val result = lib().questpdf__container__border_alignment_middle__66a27445(pointer)
    return Container(result)

}
 

 


fun borderAlignmentInside(): Container {


    val result = lib().questpdf__container__border_alignment_inside__8cef56b1(pointer)
    return Container(result)

}
 

 


fun svg(svg: String): SvgImageDescriptor {


    val result = lib().questpdf__container__svg__f547d46e(pointer, svg)
    return SvgImageDescriptor(result)

}
 

 


fun svg(image: SvgImage): SvgImageDescriptor {


    val result = lib().questpdf__container__svg__b1de06e3(pointer, image.pointer)
    return SvgImageDescriptor(result)

}
 

 


fun table(handler: (TableDescriptor) -> Unit) {

    // Create callback for handler
    val handlerCallback = object : TableDescriptorCallback {
    override fun callback(ptr: Pointer) {
    val obj = TableDescriptor(ptr)
    handler(obj)
    }
    }
    callbacks.add(handlerCallback)



    lib().questpdf__container__table__d49da987(pointer, handlerCallback)

}
 

 


fun text(content: (TextDescriptor) -> Unit) {

    // Create callback for content
    val contentCallback = object : TextDescriptorCallback {
    override fun callback(ptr: Pointer) {
    val obj = TextDescriptor(ptr)
    content(obj)
    }
    }
    callbacks.add(contentCallback)



    lib().questpdf__container__text__357e362f(pointer, contentCallback)

}
 

 


fun text(text: String): TextBlockDescriptor {


    val result = lib().questpdf__container__text__3f6b5b07(pointer, text)
    return TextBlockDescriptor(result)

}
 

 


fun translateX(value: Float, unit: LengthUnit = LengthUnit.Point): Container {


    val result = lib().questpdf__container__translate_x__351baebe(pointer, value, unit.value)
    return Container(result)

}
 

 


fun translateY(value: Float, unit: LengthUnit = LengthUnit.Point): Container {


    val result = lib().questpdf__container__translate_y__d99602db(pointer, value, unit.value)
    return Container(result)

}
 


    fun backgroundLinearGradient(angle: Float, colors: Collection<Color>): Container {
    val colorsFfi = colors.map { it.hex.toInt() }.toIntArray()
    val result = lib().questpdf__container__background_linear_gradient(pointer, angle, colorsFfi, colorsFfi.size)
    return Container(result)
}
    
fun borderLinearGradient(angle: Float, colors: Collection<Color>): Container {
    val colorsFfi = colors.map { it.hex.toInt() }.toIntArray()
    val result = lib().questpdf__container__border_linear_gradient(pointer, angle, colorsFfi, colorsFfi.size)
    return Container(result)
}
        
fun shadow(blur: Float = 0f, color: Color = Colors.Black, offsetX: Float = 0f, offsetY: Float = 0f, spread: Float = 0f): Container {
    val result = lib().questpdf__container__shadow(pointer, blur, color.hex.toInt(), offsetX, offsetY, spread)
    return Container(result)
}
}




// ============================================================================
// TableCellContainer - Native Function Interface
// ============================================================================



/**
 * JNA interface for TableCellContainer native methods.
 */
private interface TableCellContainerNative : Library {

    fun questpdf__table_cell_container__column__384372f0(target: Pointer, value: Int): Pointer

    fun questpdf__table_cell_container__column_span__629b3552(target: Pointer, value: Int): Pointer

    fun questpdf__table_cell_container__row__7ddb9999(target: Pointer, value: Int): Pointer

    fun questpdf__table_cell_container__row_span__e9016d30(target: Pointer, value: Int): Pointer



}

// ============================================================================
// TableCellContainer Callback Interfaces
// ============================================================================


// ============================================================================
// TableCellContainer Class
// ============================================================================

/**
 * TableCellContainer wrapper for the native QuestPDF TableCellContainer.
 */
class TableCellContainer internal constructor(val pointer: Pointer) {
    private val callbacks = mutableListOf<Callback>()

    companion object {
        private var nativeLib: TableCellContainerNative? = QuestLibrary.getLibrary(TableCellContainerNative::class.java)
        private val initLock = Any()

        private fun lib(): TableCellContainerNative {
            return nativeLib!!
        }

    
        
    
        
    
        
    
        
    
    }
    

 


fun column(value: Int): TableCellContainer {


    val result = lib().questpdf__table_cell_container__column__384372f0(pointer, value)
    return TableCellContainer(result)

}
 

 


fun columnSpan(value: Int): TableCellContainer {


    val result = lib().questpdf__table_cell_container__column_span__629b3552(pointer, value)
    return TableCellContainer(result)

}
 

 


fun row(value: Int): TableCellContainer {


    val result = lib().questpdf__table_cell_container__row__7ddb9999(pointer, value)
    return TableCellContainer(result)

}
 

 


fun rowSpan(value: Int): TableCellContainer {


    val result = lib().questpdf__table_cell_container__row_span__e9016d30(pointer, value)
    return TableCellContainer(result)

}
 


    
}


