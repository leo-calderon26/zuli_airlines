<template>
    <div class="flex min-h-svh flex-col bg-secondary">
        <PublicNavBar />
        <UserNavBar/>
        <main class="flex-1 bg-secondary px-6 pb-32 pt-8">
            <section class="relative mx-auto flex min-h-172.5 max-w-6xl justify-center pt-8">
                <div class="absolute left-[7%] right-[5%] top-43.75 h-px bg-purple"></div>

                <div class="relative z-10 w-full max-w-180">
                    <h2 class="mb-10 text-[15px] font-medium text-content">
                        Crear Nuevo Usuario
                    </h2>

                    <UserForm />
                </div>
            </section>
        </main>
    </div>
</template>

<script setup>
import PublicNavBar from "../../../shared/PublicNavBar.vue";
import UserNavBar from "../components/UserNavBar.vue";
import UserForm from "../components/UserForm.vue";
</script>
