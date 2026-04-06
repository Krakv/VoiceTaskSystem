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

export function DatePickerTime({ value, onChange }: DatePickerTimeProps) {
    const [open, setOpen] = React.useState(false);
    const [date, setDate] = React.useState<Date | undefined>(
        value ? new Date(value) : undefined
    );

    const handleDateSelect = (selectedDate: Date | undefined) => {
        setDate(selectedDate);
        setOpen(false);

        if (selectedDate) {
            onChange(selectedDate.toISOString());
        } else {
            onChange("");
        }
    };

    return (
        <FieldGroup className="flex flex-row gap-3 w-full">
            <Field>
                <FieldLabel htmlFor="date-picker-optional">Срок</FieldLabel>
                <Popover open={open} onOpenChange={setOpen}>
                    <PopoverTrigger asChild>
                        <Button
                            variant="outline"
                            id="date-picker-optional"
                            className="w-full justify-between font-normal"
                        >
                            {date ? format(date, "PPP") : "Выберите дату"}
                            <ChevronDownIcon />
                        </Button>
                    </PopoverTrigger>
                    <PopoverContent
                        className="w-auto overflow-hidden p-0"
                        align="start"
                    >
                        <Calendar
                            mode="single"
                            selected={date}
                            captionLayout="dropdown"
                            defaultMonth={date}
                            onSelect={handleDateSelect}
                        />
                    </PopoverContent>
                </Popover>
            </Field>
            <Field>
                <FieldLabel htmlFor="time-picker-optional">Время</FieldLabel>
                <Input
                    type="time"
                    id="time-picker-optional"
                    step="1"
                    defaultValue={date ? format(date, "HH:mm:ss") : "23:59:59"}
                    onChange={(e) => {
                        if (!date) return;
                        const [h, m, s] = e.target.value.split(":").map(Number);
                        const newDate = new Date(date);
                        newDate.setHours(h, m, s);
                        setDate(newDate);
                        onChange(newDate.toISOString());
                    }}
                    className="appearance-none bg-background [&::-webkit-calendar-picker-indicator]:hidden [&::-webkit-calendar-picker-indicator]:appearance-none w-full"
                />
            </Field>
        </FieldGroup>
    );
}