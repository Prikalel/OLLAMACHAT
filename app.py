# app.py
from flask import Flask, render_template, request, jsonify, session
from flask_session import Session  # Requires pip install Flask-Session
from chat_implementation import get_next_message, available_models
from langchain_core.messages import HumanMessage, AIMessage, SystemMessage
from gradio_client import Client
import markdown
import uuid
import threading
import time

app = Flask(__name__)
app.config["SECRET_KEY"] = "your-secret-key"
app.config["SESSION_TYPE"] = "filesystem"
Session(app)

# Initialize image client
image_client = Client("Heartsync/NSFW-Uncensored-photo",
                     hf_token="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")

class ChatSession:
    def __init__(self):
        self.messages = []
        self.model = available_models[0]
        self.active_requests = {}
        self.image_requests = {}

def format_markdown_text(text):
    return markdown.markdown(text)

@app.before_request
def initialize_session():
    if "chat_session" not in session:
        session["chat_session"] = {
            "messages": [],
            "model": available_models[0],
            "active_requests": {},
            "image_requests": {}
        }

def get_chat_session():
    return session["chat_session"]

@app.route("/")
def index():
    return render_template("index.html", models=available_models)

@app.route("/send_message", methods=["POST"])
def send_message():
    data = request.json
    user_text = data["message"].strip()
    chat_session = get_chat_session()
    
    if user_text.startswith("/img"):
        return handle_image_command(user_text[4:].strip(), chat_session)
    
    # Add user message
    chat_session["messages"].append({"role": "user", "content": user_text})
    
    # Create request ID
    request_id = str(uuid.uuid4())
    chat_session["active_requests"][request_id] = "processing"
    
    # Start processing thread
    thread = threading.Thread(target=process_message, args=(request_id, chat_session))
    thread.start()
    
    return jsonify({"request_id": request_id})

def process_message(request_id, chat_session):
    try:
        # Convert messages to langchain format
        messages = [
            HumanMessage(content=msg["content"]) if msg["role"] == "user" 
            else AIMessage(content=msg["content"])
            for msg in chat_session["messages"]
        ]
        
        content = get_next_message(chat_session["model"], messages)
        formatted_content = format_markdown_text(content)
        
        # Update session
        chat_session["messages"].append({"role": "assistant", "content": content})
        chat_session["active_requests"][request_id] = {
            "type": "message",
            "content": formatted_content
        }
    except Exception as e:
        chat_session["active_requests"][request_id] = {
            "type": "error",
            "content": f"Error: {str(e)}"
        }

@app.route("/check_response/<request_id>")
def check_response(request_id):
    chat_session = get_chat_session()
    status = chat_session["active_requests"].get(request_id, "not found")
    
    if status != "processing":
        del chat_session["active_requests"][request_id]
    
    return jsonify({
        "status": "ready" if status != "processing" else "processing",
        "response": status if status != "processing" else None
    })

def handle_image_command(prompt, chat_session):
    request_id = str(uuid.uuid4())
    chat_session["image_requests"][request_id] = "processing"
    
    # Start image generation thread
    thread = threading.Thread(target=generate_image, args=(request_id, prompt, chat_session))
    thread.start()
    
    return jsonify({"request_id": request_id})

def generate_image(request_id, user_prompt, chat_session):
    try:
        # Generate prompt if empty
        if not user_prompt.strip():
            messages = [
                SystemMessage(
                    "Create a high-detailed description in english of your character's appearance and pose based on the conversation. "
                    "Include the background as well. In response give only description in english, without any explanations or questions."
                )
            ] + [
                HumanMessage(content=msg["content"]) if msg["role"] == "user" 
                else AIMessage(content=msg["content"])
                for msg in chat_session["messages"]
            ]
            user_prompt = get_next_message(chat_session["model"], messages)
        
        # Generate image
        result = image_client.predict(
            prompt=user_prompt,
            negative_prompt="text, watermark, signature, cartoon, anime, illustration, painting, drawing, low quality, blurry",
            seed=0,
            randomize_seed=True,
            width=512,
            height=512,
            guidance_scale=4,
            num_inference_steps=28,
            api_name="/infer"
        )
        
        chat_session["image_requests"][request_id] = {
            "type": "image",
            "content": result
        }
    except Exception as e:
        chat_session["image_requests"][request_id] = {
            "type": "error",
            "content": f"Error generating image: {str(e)}"
        }

@app.route("/check_image/<request_id>")
def check_image(request_id):
    chat_session = get_chat_session()
    status = chat_session["image_requests"].get(request_id, "not found")
    
    if status != "processing":
        del chat_session["image_requests"][request_id]
    
    return jsonify({
        "status": "ready" if status != "processing" else "processing",
        "response": status if status != "processing" else None
    })

@app.route("/change_model", methods=["POST"])
def change_model():
    model = request.json["model"]
    chat_session = get_chat_session()
    chat_session["model"] = model
    return jsonify({"success": True})

if __name__ == "__main__":
    app.run(debug=True)