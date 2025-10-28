import { Link, NavLink, Outlet } from "react-router-dom";

import github from "../../assets/github.svg";
import { useLogin } from "../../authConfig";
import { LoginButton } from "../../components/LoginButton";
import styles from "./Layout.module.css";

const Layout = () => {
    return (
        <div className={styles.layout}>
            <header className={styles.header} role={"banner"}>
                <div className={styles.headerContainer}>
                    <Link to="/" className={styles.headerTitleContainer}>
                        <h3 className={styles.headerTitle}>Ejemplo de Agentes .NET</h3>
                    </Link>
                    <nav>
                        <ul className={styles.headerNavList}>
                            <li>
                                <NavLink to="/" className={({ isActive }) => (isActive ? styles.headerNavPageLinkActive : styles.headerNavPageLink)}>
                                    Chat
                                </NavLink>
                            </li>

                            <li className={styles.headerNavLeftMargin}>
                                <a href="https://github.com/dminkovski/agent-openai-banking-assistant-csharp" target={"_blank"} title="Enlace al repositorio de GitHub">
                                    <img
                                        src={github}
                                        alt="Logo de GitHub"
                                        aria-label="Enlace al repositorio de GitHub"
                                        width="20px"
                                        height="20px"
                                        className={styles.githubLogo}
                                    />
                                </a>
                            </li>
                        </ul>
                    </nav>
                    <h4 className={styles.headerRightText}>Copiloto de Asistencia Bancaria</h4>
                    {useLogin && <LoginButton />}
                </div>
            </header>

            <Outlet />
        </div>
    );
};

export default Layout;
