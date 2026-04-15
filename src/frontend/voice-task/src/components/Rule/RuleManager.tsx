import { useEffect, useState } from "react";
import { ruleApi } from "@/api/rule.api";
import { Button } from "@/components/ui/button";
import { useNavigate } from "react-router-dom";
import { Plus } from "lucide-react";
import type { RuleItem } from "@/types/rule";
import { RuleCard } from "@/components/Rule/RuleCard";
import { Skeleton } from "@/components/ui/skeleton";
import {RuleSheet} from "@/components/Rule/RuleSheet.tsx";
import {toast} from "sonner";

export const RuleManager = () => {
    const [rules, setRules] = useState<RuleItem[]>([]);
    const [selected, setSelected] = useState<RuleItem | null>(null);
    const [open, setOpen] = useState(false);
    const [loading, setLoading] = useState(true);

    const navigate = useNavigate();

    const fetchRules = async () => {
        setLoading(true);
        try {
            const res = await ruleApi.getRules();
            setRules(res.data.data.rules);
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        fetchRules();
    }, []);

    const handleToggle = async (rule: RuleItem) => {
        try {
            await ruleApi.toggleRule(rule.ruleId);

            setRules((prev) =>
                prev.map((r) =>
                    r.ruleId === rule.ruleId
                        ? { ...r, isActive: !r.isActive }
                        : r
                )
            );
        } catch {
            toast.error("Ошибка переключения правила");
        }
    };

    const handleOpen = async (rule: RuleItem) => {
        const { data } = await ruleApi.getRuleById(rule.ruleId);
        setSelected(data.data);
        setOpen(true);
    };

    const handleCreate = () => {
        navigate("/rules/create");
    };

    return (
        <div className="flex flex-col gap-2">
            {loading ? (
                <>
                    <Skeleton className="h-16 rounded-2xl" />
                    <Skeleton className="h-16 rounded-2xl" />
                    <Skeleton className="h-16 rounded-2xl" />
                </>
            ) : rules.length === 0 ? (
                <div>Правил нет</div>
            ) : (
                rules.map((rule) => (
                    <RuleCard
                        key={rule.ruleId}
                        rule={rule}
                        onOpen={handleOpen}
                        onToggle={handleToggle}
                    />
                ))
            )}

            <RuleSheet
                rule={selected}
                open={open}
                onOpenChange={setOpen}
                onEdit={(id) => navigate(`/rules/${id}/edit`)}
                onDelete={async (id) => {
                    await ruleApi.deleteRule(id);
                    setOpen(false);
                    await fetchRules();
                }}
            />

            <div className="fixed bottom-16 right-4 flex max-w-md justify-end">
                <Button
                    onClick={handleCreate}
                    className="relative rounded-full w-14 h-14 p-0 flex items-center justify-center shadow-lg bg-black text-white hover:bg-gray-600"
                >
                    <Plus className="w-10 h-10" />
                </Button>
            </div>
        </div>
    );
};