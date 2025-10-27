"""
Example usage of QuestPDF Python bindings

This demonstrates how to use the auto-generated Python wrapper for QuestPDF.
Make sure the QuestPDF native library is in the same directory or provide the path.
"""

from questpdf import QuestPDFLibrary, Handle


def create_simple_document():
    """Create a simple PDF document using QuestPDF"""
    
    # Initialize the library
    # You can pass a custom path: QuestPDFLibrary('path/to/QuestPDF.dll')
    pdf = QuestPDFLibrary()
    
    try:
        # Example: Create a container and apply styling
        # This is a conceptual example - actual method names will match your generated API
        
        # Create a document container
        container = pdf.some_container_method()
        
        # Apply alignment (example based on AlignRight method mentioned)
        with container as c:
            aligned = pdf.align_right(c)
            
            # Apply padding
            padded = pdf.padding(aligned, 10)
            
            # Add text or other content
            # result = pdf.text(padded, "Hello from Python!")
            
            print("Document created successfully!")
            
    except Exception as e:
        print(f"Error creating document: {e}")


def main():
    """Main entry point"""
    print("QuestPDF Python Example")
    print("=" * 50)
    
    try:
        create_simple_document()
    except Exception as e:
        print(f"Fatal error: {e}")


if __name__ == "__main__":
    main()

