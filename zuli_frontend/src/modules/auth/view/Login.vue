<template>
    <main class="min-h-screen flex flex-col bg-[var(--color-secondary)]">
        <!-- Header -->
        <header class="flex h-[84px] items-center justify-between bg-[var(--color-primary)] px-8 text-white">
            <!-- Logo -->
            <div class="flex w-52 items-center">
                <img
                    :src="logoZuli"
                    alt="Zuli Airlines"
                    class="h-20 w-auto object-contain"
                >
            </div>

            <!-- Título -->
            <div class="flex flex-1 justify-center">
                <h1 class="text-3xl font-bold tracking-wide">
                    Panel Administrativo
                </h1>
            </div>

            <!-- Salir -->
            <button
                type="button"
                class="flex h-full w-36 flex-col items-center justify-center border-l border-white/25"
                @click="goBack"
            >
                <img
                    :src="arrowDown"
                    alt="Salir"
                    class="h-8 w-8 rotate-90 object-contain brightness-0 invert"
                >

                <span class="mt-1 text-sm font-semibold">
                    Salir
                </span>
            </button>
        </header>

        <!-- Contenido -->
        <section class="flex flex-1 items-center justify-center px-4 py-16">
            <div class="w-full max-w-[620px] overflow-hidden rounded-b-2xl bg-white shadow-sm">
                <!-- Encabezado del panel -->
                <div class="bg-[var(--color-primary)] px-6 py-4 text-center">
                    <h2 class="text-2xl font-bold text-white">
                        Inicio de sesión
                    </h2>
                </div>

                <!-- Formulario -->
                <form class="px-12 py-12 md:px-16" @submit.prevent="login">
                    <div class="space-y-8">
                        <!-- Correo -->
                        <div class="grid grid-cols-[56px_1fr] gap-4">
                            <div class="flex justify-center pt-7">
                                <svg
                                    class="h-8 w-8 text-black"
                                    viewBox="0 0 24 24"
                                    fill="currentColor"
                                    aria-hidden="true"
                                >
                                    <path d="M20 4H4C2.9 4 2 4.9 2 6v12c0 1.1.9 2 2 2h16c1.1 0 2-.9 2-2V6c0-1.1-.9-2-2-2Zm0 4-8 5-8-5V6l8 5 8-5v2Z" />
                                </svg>
                            </div>

                            <div>
                                <label
                                    for="businessEmail"
                                    class="mb-1 block text-sm font-bold text-gray-400"
                                >
                                    Correo electrónico
                                </label>

                                <input
                                    id="businessEmail"
                                    v-model="businessEmail"
                                    type="email"
                                    autocomplete="email"
                                    class="h-10 w-full rounded-xl border border-gray-300 bg-[var(--color-body)] px-4 text-sm outline-none transition focus:border-[var(--color-primary)] focus:ring-2 focus:ring-[var(--color-primary)]/20"
                                >
                            </div>
                        </div>

                        <!-- Contraseña -->
                        <div class="grid grid-cols-[56px_1fr] gap-4">
                            <div class="flex justify-center pt-7">
                                <svg
                                    class="h-8 w-8 text-black"
                                    viewBox="0 0 24 24"
                                    fill="currentColor"
                                    aria-hidden="true"
                                >
                                    <path d="M17 8h-1V6c0-2.76-2.24-5-5-5S6 3.24 6 6v2H5c-1.1 0-2 .9-2 2v10c0 1.1.9 2 2 2h12c1.1 0 2-.9 2-2V10c0-1.1-.9-2-2-2ZM8 6c0-1.66 1.34-3 3-3s3 1.34 3 3v2H8V6Zm4 10.73V19h-2v-2.27c-.6-.35-1-.98-1-1.73 0-1.1.9-2 2-2s2 .9 2 2c0 .75-.4 1.38-1 1.73Z" />
                                </svg>
                            </div>

                            <div>
                                <label
                                    for="password"
                                    class="mb-1 block text-sm font-bold text-gray-400"
                                >
                                    Contraseña
                                </label>

                                <input
                                    id="password"
                                    v-model="password"
                                    type="password"
                                    autocomplete="current-password"
                                    class="h-10 w-full rounded-xl border border-gray-300 bg-[var(--color-body)] px-4 text-sm outline-none transition focus:border-[var(--color-primary)] focus:ring-2 focus:ring-[var(--color-primary)]/20"
                                >
                            </div>
                        </div>
                    </div>

                    <!-- Mensaje de error -->
                    <div
                        v-if="errorMessage"
                        class="mt-6 rounded-lg border border-red-200 bg-red-50 px-4 py-3 text-center text-sm font-semibold text-red-700"
                    >
                        {{ errorMessage }}
                    </div>

                    <!-- Botón -->
                    <div class="mt-10 flex justify-center">
                        <button
                            type="submit"
                            class="rounded-xl bg-[var(--color-primary)] px-12 py-3 text-lg font-bold text-white transition hover:bg-[var(--color-select)]"
                        >
                            Iniciar sesión
                        </button>
                    </div>

                    <!-- Recuperar contraseña -->
                    <div class="mt-12 text-center">
                        <button
                            type="button"
                            class="text-xs font-bold text-black hover:underline"
                            @click="recoverPassword"
                        >
                            Recuperar contraseña
                        </button>
                    </div>
                </form>
            </div>
        </section>

        <!-- Footer -->
        <footer class="h-10 bg-[var(--color-primary)]" />
    </main>
</template>

<script>
import logoZuli from "../../../assets/logoZuli.svg";
import arrowDown from "../../../assets/ArrowDown.svg";

export default {
    name: "Login",
    data() {
        return {
            logoZuli,
            arrowDown,
            businessEmail: "",
            password: "",
            errorMessage: ""
        };
    },
    methods: {
        login() {
            this.errorMessage = "";

            if (!this.businessEmail.trim()) {
                this.errorMessage = "El correo electrónico es obligatorio.";
                return;
            }

            if (!this.businessEmail.includes("@")) {
                this.errorMessage = "El correo electrónico no tiene un formato válido.";
                return;
            }

            if (!this.password) {
                this.errorMessage = "La contraseña es obligatoria.";
                return;
            }

            console.log("Formulario válido. Pendiente conexión a backend.", {
                businessEmail: this.businessEmail.trim(),
                password: this.password
            });
        },
        goBack() {
            this.businessEmail = "";
            this.password = "";
            this.errorMessage = "";
        },
        recoverPassword() {
            console.log("Recuperar contraseña pendiente de implementación.");
        }
    }
};
</script>