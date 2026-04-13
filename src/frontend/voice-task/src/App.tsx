import './App.css'
import {BrowserRouter} from "react-router-dom";
import { Toaster } from "@/components/ui/sonner";
import AppRouter from "@/components/AppRouter.tsx";

function App() {

  return (
    <>
        <Toaster position="top-right" />
        <BrowserRouter>
            <AppRouter />
        </BrowserRouter>
    </>
  )
}

export default App
