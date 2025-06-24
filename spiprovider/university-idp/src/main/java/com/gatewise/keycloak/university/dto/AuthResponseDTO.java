package com.gatewise.keycloak.university.dto;

public class AuthResponseDTO {

    private String refreshToken;
    private String accessToken;
    private String timestamp;
    private String nome;
    private Vinculo vinculo;

    public boolean isAuthenticated() {
        return accessToken != null && !accessToken.isEmpty();
    }

    public String getRefreshToken() {
        return refreshToken;
    }

    public void setRefreshToken(String refreshToken) {
        this.refreshToken = refreshToken;
    }

    public String getAccessToken() {
        return accessToken;
    }

    public void setAccessToken(String accessToken) {
        this.accessToken = accessToken;
    }

    public String getTimestamp() {
        return timestamp;
    }

    public void setTimestamp(String timestamp) {
        this.timestamp = timestamp;
    }

    public String getNome() {
        return nome;
    }

    public void setNome(String nome) {
        this.nome = nome;
    }

    public Vinculo getVinculo() {
        return vinculo;
    }

    public void setVinculo(Vinculo vinculo) {
        this.vinculo = vinculo;
    }

    public static class Vinculo {

        private int id;
        private int tipo;
        private String nome;
        private String descricao;
        private String matricula;
        private String email;
        private String foto;
        private int unidadeId;
        private String nomeUnidade;
        private int cursoId;
        private int anoIngresso;
        private String descricaoSituacao;

        public int getId() {
            return id;
        }

        public void setId(int id) {
            this.id = id;
        }

        public int getTipo() {
            return tipo;
        }

        public void setTipo(int tipo) {
            this.tipo = tipo;
        }

        public String getNome() {
            return nome;
        }

        public void setNome(String nome) {
            this.nome = nome;
        }

        public String getDescricao() {
            return descricao;
        }

        public void setDescricao(String descricao) {
            this.descricao = descricao;
        }

        public String getMatricula() {
            return matricula;
        }

        public void setMatricula(String matricula) {
            this.matricula = matricula;
        }

        public String getEmail() {
            return email;
        }

        public void setEmail(String email) {
            this.email = email;
        }

        public String getFoto() {
            return foto;
        }

        public void setFoto(String foto) {
            this.foto = foto;
        }

        public int getUnidadeId() {
            return unidadeId;
        }

        public void setUnidadeId(int unidadeId) {
            this.unidadeId = unidadeId;
        }

        public String getNomeUnidade() {
            return nomeUnidade;
        }

        public void setNomeUnidade(String nomeUnidade) {
            this.nomeUnidade = nomeUnidade;
        }

        public int getCursoId() {
            return cursoId;
        }

        public void setCursoId(int cursoId) {
            this.cursoId = cursoId;
        }

        public int getAnoIngresso() {
            return anoIngresso;
        }

        public void setAnoIngresso(int anoIngresso) {
            this.anoIngresso = anoIngresso;
        }

        public String getDescricaoSituacao() {
            return descricaoSituacao;
        }

        public void setDescricaoSituacao(String descricaoSituacao) {
            this.descricaoSituacao = descricaoSituacao;
        }
    }

    public static class AffiliationType {

        public static final int VISITOR = 0;
        public static final int STUDENT = 1;
        public static final int PROFESSOR = 2;
        public static final int TECHNICAL_STAFF = 3;
        public static final int SERVICE_PROVIDER_PROFESSOR = 4;
        public static final int SERVICE_PROVIDER = 5;
        public static final int INTERN = 6;

        public static String getDescription(int type) {
            switch (type) {
                case STUDENT:
                    return "Student";
                case PROFESSOR:
                    return "Professor";
                case TECHNICAL_STAFF:
                    return "Technical Staff";
                case SERVICE_PROVIDER_PROFESSOR:
                    return "Service Provider (Professor)";
                case SERVICE_PROVIDER:
                    return "Service Provider";
                case INTERN:
                    return "Intern";
                case VISITOR:
                    return "Visitor";
                default:
                    return "Unknown";
            }
        }
    }

}
