import sys
import markdown
from PyQt5.QtWidgets import (QApplication, QMainWindow, QWidget, QVBoxLayout, 
                            QHBoxLayout, QTextEdit, QPushButton, 
                            QTabWidget, QLabel, QComboBox, QShortcut)
from PyQt5.QtCore import Qt, QThread, pyqtSignal, QTimer
from PyQt5.QtGui import QFont, QKeySequence, QPalette, QColor
from langchain_core.messages import HumanMessage, AIMessage, SystemMessage
from gradio_client import Client
from PyQt5.QtGui import QPixmap
from PyQt5.QtCore import Qt, QThread, pyqtSignal, QTimer, QUrl, QVariant
from PyQt5.QtGui import QFont, QKeySequence, QPalette, QColor, QPixmap, QTextDocument
from chat_implementation import get_next_message, available_models

class ChatWorker(QThread):
    response_ready = pyqtSignal(str)
    
    def __init__(self, model, messages):
        super().__init__()
        self.model = model
        self.messages = messages.copy()
        
    def run(self):
        try:
            content = get_next_message(self.model, self.messages)
            self.response_ready.emit(content)
        except Exception as e:
            self.response_ready.emit(f"Error: {str(e)}")

def format_markdown_text(text):
    return markdown.markdown(text)

class ChatTab(QWidget):
    def __init__(self, parent=None, model=available_models[0]):
        super().__init__(parent)
        self.messages = []
        self.model = model
        self.thinking_dots = 0
        self.thinking_timer = QTimer()
        self.thinking_timer.timeout.connect(self.update_thinking_dots)
        self.init_ui()
        self.image_client = Client("Heartsync/NSFW-Uncensored-photo", 
                                 hf_token="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")
        
    def init_ui(self):
        layout = QVBoxLayout(self)

        # Model selection
        model_layout = QHBoxLayout()
        self.model_combo = QComboBox()
        self.model_combo.addItems(available_models)
        self.model_combo.setCurrentText(self.model)
        self.model_combo.currentTextChanged.connect(self.change_model)
        model_layout.addWidget(QLabel("Model:"))
        model_layout.addWidget(self.model_combo)
        model_layout.addStretch()
        layout.addLayout(model_layout)

        # Chat display
        self.chat_display = QTextEdit()
        self.chat_display.setReadOnly(True)
        font = QFont("Consolas", 10)
        self.chat_display.setFont(font)
        layout.addWidget(self.chat_display)

        # Input area
        input_layout = QHBoxLayout()
        self.input_field = QTextEdit()
        self.input_field.setMaximumHeight(80)
        self.input_field.setPlaceholderText("Type your message here...")

        self.send_button = QPushButton("Send")
        self.send_button.clicked.connect(self.send_message)
        self.send_button.setDefault(True)

        input_layout.addWidget(self.input_field)
        input_layout.addWidget(self.send_button)
        layout.addLayout(input_layout)

        # Set up keyboard shortcut for sending
        self.shortcut = QShortcut(QKeySequence("Ctrl+Return"), self)
        self.shortcut.activated.connect(self.send_message)

    def change_model(self, model_name):
        self.model = model_name
        self.chat_display.append(f"<i>Model changed to {model_name}</i>")

    def send_message(self):
        user_text = self.input_field.toPlainText().strip()
        if not user_text:
            return

        # Handle /img command
        if user_text.startswith("/img"):
            self.generate_visual_novel_frame_based_on_chat_messages(user_text[4:].strip())
            return

        # Display user message with oceanic-light-blue color
        self.chat_display.append("<b style='color:#4FC3F7;'>You:</b> {0}".format(user_text))
        self.input_field.clear()

        # Add to messages
        self.messages.append(HumanMessage(content=user_text))

        # Indicate that the AI is thinking with light-red color
        self.thinking_line_position = self.chat_display.document().characterCount()
        self.chat_display.append("<b style='color:#FF8A80;'>AI:</b> <i>Thinking...</i>")

        # Disable the send button while thinking
        self.send_button.setEnabled(False)
        self.input_field.setReadOnly(True)

        # Start the thinking animation
        self.thinking_dots = 0
        self.thinking_timer.start(500)

        # Process in background thread
        self.worker = ChatWorker(self.model, self.messages)
        self.worker.response_ready.connect(self.handle_response)
        self.worker.start()

    def generate_visual_novel_frame_based_on_chat_messages(self, user_custom_promt: str):
        # Display processing message
        self.chat_display.append("<b style='color:#4FC3F7;'>You:</b> /img " + user_custom_promt)
        self.chat_display.append("<b style='color:#FF8A80;'>AI:</b> <i style='color:#A9A9A9;'>Generating image from last message...</i>")
        self.input_field.clear()

        # Disable UI during generation
        self.send_button.setEnabled(False)
        self.input_field.setReadOnly(True)

        # Process in background thread
        self.img_worker = ImageWorker(self.image_client, self.messages, self.model, user_custom_promt)
        self.img_worker.image_ready.connect(self.handle_image_response)
        self.img_worker.start()

    def handle_image_response(self, image_path):
        # Stop any processing display if it exists
        self.thinking_timer.stop()

        # Create image label
        pixmap = QPixmap(image_path)
        
        # Add to chat display
        self.chat_display.document().addResource(
            QTextDocument.ImageResource,
            QUrl(image_path),
            QVariant(pixmap)
        )
        
        # Ensure we're at the end of the document
        cursor = self.chat_display.textCursor()
        cursor.movePosition(cursor.End)
        
        # Add a line break before the image
        cursor.insertText("\n")
        
        # Insert the image at the current (end) position
        cursor.insertImage(image_path)
        
        # Add a line break after the image
        cursor.insertText("\n")
        
        # Re-enable UI
        self.send_button.setEnabled(True)
        self.input_field.setReadOnly(False)
        
        # Scroll to bottom to ensure the image is visible
        self.chat_display.verticalScrollBar().setValue(
            self.chat_display.verticalScrollBar().maximum()
        )

    def update_thinking_dots(self):
        # Update the thinking dots animation
        self.thinking_dots = (self.thinking_dots + 1) % 4
        dots = "." * self.thinking_dots

        # Get the current text cursor
        cursor = self.chat_display.textCursor()

        # Move to the end of the document
        cursor.movePosition(cursor.End)

        # Select and remove the current line
        cursor.movePosition(cursor.StartOfLine, cursor.KeepAnchor)
        cursor.removeSelectedText()

        # Insert the updated thinking message with light-red color
        cursor.insertHtml("<b style='color:#FF8A80;'>AI:</b> <i>Thinking{0}</i>".format(dots))

        # Scroll to bottom
        self.chat_display.verticalScrollBar().setValue(
            self.chat_display.verticalScrollBar().maximum()
        )

    def handle_response(self, content):
        # Stop the thinking animation
        self.thinking_timer.stop()

        # Remove the "thinking" message
        cursor = self.chat_display.textCursor()
        cursor.movePosition(cursor.End)
        cursor.select(cursor.LineUnderCursor)
        cursor.removeSelectedText()

        # Format the content with markdown
        formatted_content = format_markdown_text(content)

        # Add the real response with light-red color
        self.chat_display.append("<b style='color:#FF8A80;'>AI:</b> {0}".format(formatted_content))
        self.messages.append(AIMessage(content=content))

        # Re-enable the send button
        self.send_button.setEnabled(True)
        self.input_field.setReadOnly(False)

        # Scroll to bottom
        self.chat_display.verticalScrollBar().setValue(
            self.chat_display.verticalScrollBar().maximum()
        )

class ChatApp(QMainWindow):
    def __init__(self):
        super().__init__()
        self.setWindowTitle("Multi-Chat Application")
        self.setGeometry(100, 100, 800, 600)
        self.set_dark_theme()
        self.init_ui()

    def set_dark_theme(self):
        # Set application style to dark
        dark_palette = QPalette()

        # Set colors for different states and roles
        dark_palette.setColor(QPalette.Window, QColor(53, 53, 53))
        dark_palette.setColor(QPalette.WindowText, Qt.white)
        dark_palette.setColor(QPalette.Base, QColor(25, 25, 25))
        dark_palette.setColor(QPalette.AlternateBase, QColor(53, 53, 53))
        dark_palette.setColor(QPalette.ToolTipBase, Qt.black)
        dark_palette.setColor(QPalette.ToolTipText, Qt.white)
        dark_palette.setColor(QPalette.Text, Qt.white)
        dark_palette.setColor(QPalette.Button, QColor(53, 53, 53))
        dark_palette.setColor(QPalette.ButtonText, Qt.white)
        dark_palette.setColor(QPalette.BrightText, Qt.red)
        dark_palette.setColor(QPalette.Link, QColor(42, 130, 218))
        dark_palette.setColor(QPalette.Highlight, QColor(42, 130, 218))
        dark_palette.setColor(QPalette.HighlightedText, Qt.black)

        # Apply palette
        self.setPalette(dark_palette)

    def init_ui(self):
        # Central widget and layout
        self.central_widget = QWidget()
        self.setCentralWidget(self.central_widget)
        main_layout = QVBoxLayout(self.central_widget)
        
        # Tab widget for multiple chats
        self.tabs = QTabWidget()
        self.tabs.setTabsClosable(True)
        self.tabs.tabCloseRequested.connect(self.close_tab)
        main_layout.addWidget(self.tabs)
        
        # Add initial tab
        self.add_chat_tab()
        
        # Buttons layout
        btn_layout = QHBoxLayout()
        
        # New chat button
        new_chat_btn = QPushButton("New Chat")
        new_chat_btn.clicked.connect(self.add_chat_tab)
        btn_layout.addWidget(new_chat_btn)
        
        # Add layout to main layout
        main_layout.addLayout(btn_layout)
        
    def add_chat_tab(self):
        chat_tab = ChatTab(self)
        tab_index = self.tabs.addTab(chat_tab, f"Chat {self.tabs.count() + 1}")
        self.tabs.setCurrentIndex(tab_index)
        
    def close_tab(self, index):
        if self.tabs.count() > 1:
            self.tabs.removeTab(index)

class ImageWorker(QThread):
    image_ready = pyqtSignal(str)
    
    def __init__(self, client, messages, llm: str, user_custom_promt):
        super().__init__()
        self.client = client
        self.messages = messages
        self.llm = llm
        self.user_custom_promt = user_custom_promt
        
    def run(self):
        try:
            # Generate image description
            prompt = self.user_custom_promt if not self.user_custom_promt.isspace() and len(self.user_custom_promt) > 0 else get_next_message(self.llm, 
                self.messages + [SystemMessage(
                    "Create a high-detailed description in english of your character's appearance and pose based on the conversation. " \
                    "Include the background as well. In response give only description in english, without any explanations or questions.")])
            
            print("The image will be generated prompt: " + prompt)
            # Generate image
            result = self.client.predict(
                prompt=prompt,
                negative_prompt="text, watermark, signature, cartoon, anime, illustration, painting, drawing, low quality, blurry",
                seed=0,
                randomize_seed=True,
                width=512,
                height=512,
                guidance_scale=4,
                num_inference_steps=28,
                api_name="/infer"
            )
            self.image_ready.emit(result)
        except Exception as e:
            self.image_ready.emit(f"Error generating image: {str(e)}")

if __name__ == "__main__":
    app = QApplication(sys.argv)

    # Setting application-wide dark theme
    app.setStyle("Fusion")
    dark_palette = QPalette()
    dark_palette.setColor(QPalette.Window, QColor(53, 53, 53))
    dark_palette.setColor(QPalette.WindowText, Qt.white)
    dark_palette.setColor(QPalette.Base, QColor(0, 0, 0))  # Black for text areas
    dark_palette.setColor(QPalette.AlternateBase, QColor(53, 53, 53))
    dark_palette.setColor(QPalette.ToolTipBase, Qt.black)
    dark_palette.setColor(QPalette.ToolTipText, Qt.white)
    dark_palette.setColor(QPalette.Text, Qt.white)
    dark_palette.setColor(QPalette.Button, QColor(53, 53, 53))
    dark_palette.setColor(QPalette.ButtonText, Qt.white)
    dark_palette.setColor(QPalette.BrightText, Qt.red)
    dark_palette.setColor(QPalette.Link, QColor(42, 130, 218))
    dark_palette.setColor(QPalette.Highlight, QColor(42, 130, 218))
    dark_palette.setColor(QPalette.HighlightedText, Qt.black)
    dark_palette.setColor(QPalette.Disabled, QPalette.Text, QColor(127, 127, 127))
    dark_palette.setColor(QPalette.Disabled, QPalette.ButtonText, QColor(127, 127, 127))
    app.setPalette(dark_palette)

    window = ChatApp()
    window.show()
    sys.exit(app.exec_())