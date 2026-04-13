import { useParams } from "react-router-dom";
import { useEffect, useState } from "react";
import { ruleApi } from "@/api/rule.api";
import { RuleForm } from "@/components/Rule/RuleForm";
import type { RuleItem } from "@/types/rule";

export const RuleEditPage = () => {
    const { ruleId } = useParams<{ ruleId: string }>();
    const [rule, setRule] = useState<RuleItem | undefined>(undefined);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        if (!ruleId) return;

        ruleApi.getRuleById(ruleId)
            .then((res) => {
                setRule(res.data.data);
            })
            .finally(() => setLoading(false));
    }, [ruleId]);

    return (
        <>
            {loading ? (
                <div>Загрузка...</div>
            ) : (
                <RuleForm rule={rule} />
            )}
        </>
    );
};