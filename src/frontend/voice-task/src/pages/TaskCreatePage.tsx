import { TaskForm } from "@/components/TaskForm";
import {useLocation} from "react-router-dom";
import {VoiceInputPage} from "@/pages/VoiceInputPage.tsx";

export const TaskCreatePage = () => {
    const { pathname } = useLocation();

    if (pathname === "/create") {
        return <TaskForm/>
    } else {
        return <VoiceInputPage/>
    }
};