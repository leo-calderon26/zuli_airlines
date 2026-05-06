<template>
    <div class="flex min-h-svh flex-col bg-[var(--color-secondary)]">
        <PublicNavBar />

        <main class="flex flex-1 flex-col items-center justify-center gap-6 px-6 py-16">
            <h1 class="text-[28px] font-bold text-black">
                Users en mantenimiento
            </h1>

            <button
                class="h-11 min-w-[180px] rounded-lg bg-[var(--color-primary)] px-6 font-bold text-white transition hover:cursor-pointer hover:brightness-75"
                type="button"
                @click="goToCreateUser"
            >
                Agregar usuario
            </button>
        </main>
    </div>
</template>

<script>
import PublicNavBar from "../../../shared/PublicNavBar.vue";

export default {
    name: "UserList",

    components: {
        PublicNavBar
    },

    methods: {
        goToCreateUser() {
            this.$router.push({ name: "userCreate" });
        }
    }
};
</script>