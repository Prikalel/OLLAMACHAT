from typing import List
from langchain_llm7 import ChatLLM7
from langchain_core.messages import BaseMessage
import json

available_models = ["deepseek-v3", "gpt-4.1", "bidara", "deepseek-r1", "mirexa", "sur", "gpt-4.1-mini"]

def get_next_message(model: str, messages: List[BaseMessage]) -> str:
    llm = ChatLLM7(model=model)
    response = llm.invoke(messages)
    # for deepseek-v3 we need to decode the response with json
    if model == "deepseek-v3":
        return json.loads(response.content)["reasoning_content"]
    return response.content
