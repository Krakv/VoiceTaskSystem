import { useState, useRef, useCallback } from "react";
import { Button } from "@/components/ui/button.tsx";
import { Mic, Send, X } from "lucide-react";
import { commandRequestApi } from "@/api/commandRequest.api.ts";
import { useNavigate } from "react-router-dom";

type ScreenState = "idle" | "recording" | "error";

function WaveBars() {
    return (
        <>
            <style>{`
                @keyframes voiceBar { to { transform: scaleY(0.25); } }
                @keyframes ringPulse { 0%{transform:scale(1);opacity:.5} 100%{transform:scale(1.9);opacity:0} }
            `}</style>
            <div className="flex gap-[3px] items-end">
                {[10, 20, 28, 18, 10].map((h, i) => (
                    <span
                        key={i}
                        className="w-[4px] rounded bg-red-500"
                        style={{
                            height: h,
                            animation: `voiceBar 0.7s ${i * 0.1}s ease-in-out infinite alternate`,
                        }}
                    />
                ))}
            </div>
        </>
    );
}

export const VoiceInputPage = () => {
    const [screen, setScreen] = useState<ScreenState>("idle");
    const [error, setError] = useState("");

    const mediaRecorderRef = useRef<MediaRecorder | null>(null);
    const chunksRef = useRef<Blob[]>([]);

    const navigate = useNavigate();

    const uploadAudio = async (blob: Blob): Promise<string> => {
        const res = await commandRequestApi.createVoiceTask(blob);
        return res.data.data.commandRequestId;
    };

    const startRecording = async () => {
        try {
            const stream = await navigator.mediaDevices.getUserMedia({ audio: true });
            const mr = new MediaRecorder(stream);

            chunksRef.current = [];

            mr.ondataavailable = (e) => {
                if (e.data.size > 0) chunksRef.current.push(e.data);
            };

            mr.start();
            mediaRecorderRef.current = mr;
            setScreen("recording");
        } catch {
            setError("Нет доступа к микрофону");
            setScreen("error");
        }
    };

    const stopRecording = () => {
        const mr = mediaRecorderRef.current;
        if (!mr) return;

        mr.stop();
        mr.stream.getTracks().forEach((t) => t.stop());

        chunksRef.current = [];
        setScreen("idle");
    };

    const stopAndSend = useCallback(() => {
        const mr = mediaRecorderRef.current;
        if (!mr) return;

        mr.onstop = async () => {
            const blob = new Blob(chunksRef.current, { type: "audio/webm" });
            mr.stream.getTracks().forEach((t) => t.stop());

            try {
                const reqId = await uploadAudio(blob);

                navigate(`/create/voice/${reqId}`);
            } catch {
                setError("Ошибка загрузки аудио");
                setScreen("error");
            }
        };

        mr.stop();
    }, [navigate, setError, setScreen]);

    function handleCreateManually() {
        navigate("/create");
    }

    return (
        <div className="flex flex-col h-full items-center justify-center gap-6">
            <div className="flex justify-items-end">
                <Button variant={"outline"} onClick={handleCreateManually}>Создать вручную</Button>
            </div>

            {screen === "idle" && (
                <div className="flex flex-col flex-1 items-center justify-center gap-5">
                    <div className="flex flex-col items-center gap-3 text-center">
                        <div className="w-20 h-20 rounded-full bg-muted flex items-center justify-center">
                            <Mic className="w-8 h-8 text-muted-foreground" />
                        </div>
                        <p className="text-sm text-muted-foreground max-w-[200px] leading-snug">
                            Нажмите кнопку и продиктуйте команду
                        </p>
                    </div>
                    <Button size="lg" className="rounded-full px-8 gap-2" onClick={startRecording}>
                        <Mic className="w-4 h-4" />
                        Начать запись
                    </Button>
                </div>
            )}

            {screen === "recording" && (
                <div className="flex flex-col flex-1 items-center justify-center gap-6">
                    <div className="relative flex items-center justify-center w-24 h-24 rounded-full border-2 border-red-300 bg-red-50">
                            <span className="absolute w-24 h-24 rounded-full border border-red-300"
                                  style={{ animation: "ringPulse 1.4s ease-out infinite" }} />
                        <span className="absolute w-24 h-24 rounded-full border border-red-300"
                              style={{ animation: "ringPulse 1.4s 0.5s ease-out infinite" }} />
                        <WaveBars />
                    </div>
                    <div className="text-center text-sm text-muted-foreground leading-snug">
                        Слушаю вас…<br />Говорите команду
                    </div>
                    <div className="flex gap-4">
                        <Button variant="destructive" onClick={stopRecording}>
                            <X /> Отмена
                        </Button>
                        <Button onClick={stopAndSend}>
                            <Send /> Отправить
                        </Button>
                    </div>
                </div>
            )}

            {screen === "error" && (
                <div className="text-center">
                    <p>{error}</p>
                    <Button onClick={() => setScreen("idle")}>Назад</Button>
                </div>
            )}
        </div>
    );
};