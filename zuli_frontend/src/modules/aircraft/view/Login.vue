<template>
    <main class="min-h-screen flex flex-col bg-[#d9d9d9]">
        <!-- Header -->
        <header class="flex items-center justify-between bg-[#8B1111] px-6 py-4 text-white">
            <div class="flex items-center gap-4">
                <!-- Placeholder logo -->
                <div class="flex h-14 w-32 items-center justify-center rounded bg-transparent text-sm font-bold text-[#f4d27a]">
                    [LOGO ZULI]
                </div>
            </div>

            <div class="flex-1 text-center">
                <h1 class="text-2xl font-bold md:text-3xl">
                    Panel Administrativo
                </h1>
            </div>

            <div class="flex w-28 flex-col items-center justify-center border-l border-white/40 pl-4">
                <!-- Placeholder icon salir -->
                <div class="mb-1 flex h-8 w-8 items-center justify-center rounded text-white">
                    [←]
                </div>
                <span class="text-sm font-semibold">Salir</span>
            </div>
        </header>

        <!-- Content -->
        <section class="flex flex-1 items-center justify-center px-4 py-10">
            <div class="w-full max-w-2xl overflow-hidden rounded-2xl bg-[#f5f5f5] shadow-md">
                <!-- Título caja -->
                <div class="bg-[#8B1111] px-6 py-5 text-center">
                    <h2 class="text-3xl font-bold text-white">
                        Inicio de sesión
                    </h2>
                </div>

                <!-- Form -->
                <div class="px-8 py-12 md:px-16">
                    <form class="space-y-8" @submit.prevent="login">
                        <!-- Correo -->
                        <div class="flex items-center gap-4">
                            <!-- Placeholder icon correo -->
                            <div class="flex h-10 w-10 items-center justify-center text-xl text-black">
                                [ICONO]
                            </div>

                            <div class="flex-1">
                                <label
                                    for="businessEmail"
                                    class="mb-1 block text-sm font-semibold text-gray-400"
                                >
                                    Correo electrónico
                                </label>

                                <input
                                    id="businessEmail"
                                    v-model="businessEmail"
                                    type="email"
                                    class="w-full rounded-xl border border-gray-300 bg-white px-4 py-2 outline-none transition focus:border-[#8B1111] focus:ring-2 focus:ring-[#8B1111]/20"
                                >
                            </div>
                        </div>

                        <!-- Contraseña -->
                        <div class="flex items-center gap-4">
                            <!-- Placeholder icon contraseña -->
                            <div class="flex h-10 w-10 items-center justify-center text-xl text-black">
                                [ICONO]
                            </div>

                            <div class="flex-1">
                                <label
                                    for="password"
                                    class="mb-1 block text-sm font-semibold text-gray-400"
                                >
                                    Contraseña
                                </label>

                                <input
                                    id="password"
                                    v-model="password"
                                    type="password"
                                    class="w-full rounded-xl border border-gray-300 bg-white px-4 py-2 outline-none transition focus:border-[#8B1111] focus:ring-2 focus:ring-[#8B1111]/20"
                                >
                            </div>
                        </div>

                        <!-- Error -->
                        <div
                            v-if="errorMessage"
                            class="rounded-lg border border-red-200 bg-red-50 px-4 py-3 text-sm text-red-700"
                        >
                            {{ errorMessage }}
                        </div>

                        <!-- Botón -->
                        <div class="flex justify-center pt-2">
                            <button
                                type="submit"
                                class="rounded-xl bg-[#8B1111] px-10 py-3 text-xl font-bold text-white transition hover:bg-[#6f0d0d]"
                            >
                                Iniciar sesión
                            </button>
                        </div>

                        <!-- Recuperar contraseña -->
                        <div class="pt-6 text-center">
                            <button
                                type="button"
                                class="text-sm font-semibold text-black hover:underline"
                            >
                                Recuperar contraseña
                            </button>
                        </div>
                    </form>
                </div>
            </div>
        </section>

        <!-- Footer -->
        <footer class="h-10 bg-[#8B1111]" />
    </main>
</template>

<script>
export default {
    name: "Login",
    data() {
        return {
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

            // TODO:
            // Aquí luego se conectará el backend.
            // Ejemplo futuro:
            // fetch('/api/auth/login', { ... })

            console.log("Formulario válido. Aquí se conectará el backend después.", {
                businessEmail: this.businessEmail,
                password: this.password
            });
        }
    }
};
</script>