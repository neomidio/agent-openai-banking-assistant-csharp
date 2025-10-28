import { Example } from "./Example";

import styles from "./Example.module.css";

export type ExampleModel = {
    text: string;
    value: string;
};

const EXAMPLES: ExampleModel[] = [
    { text: "Quiero pagar el recibo de electricidad", value: "Quiero pagar el recibo de electricidad"},
    { text: "¿Qué pagos hice este mes?", value: "¿Qué pagos hice este mes?" },
    { text: "¿Cuál es el cupo disponible de mi tarjeta Visa?", value: "¿Cuál es el cupo disponible de mi tarjeta Visa?" }
];

interface Props {
    onExampleClicked: (value: string) => void;
}

export const ExampleList = ({ onExampleClicked }: Props) => {
    return (
        <ul className={styles.examplesNavList}>
            {EXAMPLES.map((x, i) => (
                <li key={i}>
                    <Example text={x.text} value={x.value} onClick={onExampleClicked} />
                </li>
            ))}
        </ul>
    );
};
