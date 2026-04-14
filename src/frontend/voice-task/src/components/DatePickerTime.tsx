import * as React from "react";
import { format } from "date-fns";
import { ChevronDownIcon } from "lucide-react";

import { Button } from "@/components/ui/button";
import { Calendar } from "@/components/ui/calendar";
import { Field, FieldGroup, FieldLabel } from "@/components/ui/field";
import { Input } from "@/components/ui/input";
import {
    Popover,
    PopoverContent,
    PopoverTrigger,
} from "@/components/ui/popover";

interface DatePickerTimeProps {
    value?: string;
    onChange: (value: string) => void;
}

const toIsoWithOffset = (date: Date) => {
    const pad = (n: number) => String(n).padStart(2, "0");

    const tzOffset = -date.getTimezoneOffset(); // минуты
    const sign = tzOffset >= 0 ? "+" : "-";

    const hoursOffset = pad(Math.floor(Math.abs(tzOffset) / 60));
    const minutesOffset = pad(Math.abs(tzOffset) % 60);

    return `${date.getFullYear()}-${pad(date.getMonth() + 1)}-${pad(date.getDate())}T${pad(date.getHours())}:${pad(date.getMinutes())}:00${sign}${hoursOffset}:${minutesOffset}`;
};

export function DatePickerTime({ value, onChange }: DatePickerTimeProps) {
    const [open, setOpen] = React.useState(false);
    const [date, setDate] = React.useState<Date | undefined>();

    React.useEffect(() => {
        if (value) {
            setDate(new Date(value));
        }
    }, [value]);

    const handleDateSelect = (selectedDate: Date | undefined) => {
        if (!selectedDate) {
            setDate(undefined);
            onChange("");
            return;
        }

        // сохраняем текущее время (если было)
        const newDate = new Date(selectedDate);
        if (date) {
            newDate.setHours(date.getHours(), date.getMinutes());
        }

        setDate(newDate);
        setOpen(false);
        onChange(toIsoWithOffset(newDate));
    };

    const handleTimeChange = (time: string) => {
        if (!date) return;

        const [h, m] = time.split(":").map(Number);

        const newDate = new Date(date);
        newDate.setHours(h, m, 0, 0);

        setDate(newDate);
        onChange(toIsoWithOffset(newDate));
    };

    return (
        <FieldGroup className="flex flex-row gap-3 w-full">
            <Field>
                <FieldLabel>Дата</FieldLabel>
                <Popover open={open} onOpenChange={setOpen}>
                    <PopoverTrigger asChild>
                        <Button
                            variant="outline"
                            className="w-full justify-between font-normal"
                        >
                            {date ? format(date, "PPP") : "Выберите дату"}
                            <ChevronDownIcon />
                        </Button>
                    </PopoverTrigger>
                    <PopoverContent className="w-auto p-0" align="start">
                        <Calendar
                            mode="single"
                            selected={date}
                            onSelect={handleDateSelect}
                        />
                    </PopoverContent>
                </Popover>
            </Field>

            <Field>
                <FieldLabel>Время</FieldLabel>
                <Input
                    type="time"
                    value={date ? format(date, "HH:mm") : ""}
                    onChange={(e) => handleTimeChange(e.target.value)}
                    className="w-full"
                />
            </Field>
        </FieldGroup>
    );
}