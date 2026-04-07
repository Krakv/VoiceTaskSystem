import type {TaskUpdatePayload} from "@/types/commandRequest.ts";
import {api} from "@/api/http.ts";


export const commandRequestApi = {
    createVoiceTask: (file: File | Blob) => {
        const formData = new FormData();
        formData.append("FormFile", file, "recording.webm");
        return api.post("/tasks/voice", formData, {
            headers: { "Content-Type": "multipart/form-data" },
        });
    },
    getVoiceTaskStatus: (requestId: string) => api.get(`/tasks/voice/requests/${requestId}/status`),
    patchVoiceTask: (requestId: string, data: TaskUpdatePayload) => api.patch(`/tasks/voice/requests/${requestId}`, data),
    confirmVoiceTask: (requestId: string) => api.post(`/tasks/voice/requests/${requestId}/confirm`),
    deleteVoiceTask: (requestId: string) => api.delete(`/tasks/voice/requests/${requestId}`),
}